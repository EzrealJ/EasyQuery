using Ezreal.EasyQuery.Enums;
using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ezreal.EasyQuery.Model
{


    public class WhereCondition
    {
        /// <summary>
        /// 搜索列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 搜索值【简单类型值无需处理，序列化传递时以Json字符串序列化】
        /// <para>
        /// <see cref="EnumMatchMode.Between"/>和<see cref="EnumMatchMode.NotBetween"/>需要传递一个2元素的数组[Start,End] 以Json序列化的方式处理
        /// </para>
        /// <para>
        /// <see cref="EnumMatchMode.In"/>和<see cref="EnumMatchMode.NotIn"/>需要传递一个N元素的数组[item1,item2...itemN] 以Json序列化的方式处理
        /// </para>
        /// <para>
        /// *以Json序列化搜索值，以适应特殊字符
        /// </para>
        /// </summary>
        public object ColumnValue { get; set; }
        /// <summary>
        /// 匹配模式
        /// </summary>
        public EnumMatchMode MatchMode { get; set; }

        //public StringComparison StringComparison { get; set; }

        private static readonly MethodInfo _objectToStringMethod = typeof(object).GetMethod(nameof(object.ToString), new Type[] { });
        private static readonly MethodInfo _stringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
        private static readonly MethodInfo _stringStartsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });
        private static readonly MethodInfo _stringEndsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });
        private static readonly MethodInfo _stringToLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), new Type[] { });
        private static readonly MethodInfo _enumerableContainsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

        public WhereCondition(string columnName, object columnValue, EnumMatchMode matchMode)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            ColumnValue = columnValue ?? throw new ArgumentNullException(nameof(columnValue));
            MatchMode = matchMode;
        }

        public WhereCondition()
        {
        }

        /// <summary>
        /// 获取表达式
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual Expression GetExpression<TSource>(ParameterExpression parameter)
        {

            MemberExpression member = Expression.PropertyOrField(parameter, this.ColumnName);
            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (this.ColumnValue != null)
                {
                    member = Expression.Property(member, "Value");
                }
            }
            if (this.MatchMode == EnumMatchMode.Equal)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.Equal(member, constant);
            }
            if (this.MatchMode == EnumMatchMode.NotEqual)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.NotEqual(member, constant);
            }
            if (this.MatchMode == EnumMatchMode.Like)
            {
                Expression constant = ConstantExpression(this.ColumnValue.ToString());
                Expression currentExpression = member.Type == typeof(string) ? (Expression)member : Expression.Call(member, _objectToStringMethod);
                currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
                currentExpression = Expression.Call(currentExpression, _stringContainsMethod, Expression.Call(constant, _stringToLowerMethod));
                return AttachNotNullCheckExpression(member, currentExpression);

            }
            if (this.MatchMode == EnumMatchMode.NotLike)
            {
                Expression constant = ConstantExpression(this.ColumnValue.ToString());
                Expression currentExpression = member.Type == typeof(string) ? (Expression)member : Expression.Call(member, _objectToStringMethod);
                currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
                currentExpression = Expression.Call(currentExpression, _stringContainsMethod, Expression.Call(constant, _stringToLowerMethod));
                currentExpression = Expression.Not(currentExpression);
                return AttachNotNullCheckExpression(member, currentExpression);

            }
            if (this.MatchMode == EnumMatchMode.Less)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.LessThan(member, constant);

            }
            if (this.MatchMode == EnumMatchMode.LessOrEqual)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.LessThanOrEqual(member, constant);
            }
            if (this.MatchMode == EnumMatchMode.More)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.GreaterThan(member, constant);

            }
            if (this.MatchMode == EnumMatchMode.MoreOrEqual)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.GreaterThanOrEqual(member, constant);
            }


            if (this.MatchMode == EnumMatchMode.Between)
            {

                object[] valueArray = this.ColumnValue as object[];
                Expression constantStart = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                Expression constantEnd = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                return Expression.AndAlso(Expression.GreaterThanOrEqual(member, constantStart), Expression.LessThanOrEqual(member, constantEnd));

            }
            if (this.MatchMode == EnumMatchMode.NotBetween)
            {
                object[] valueArray = this.ColumnValue as object[];
                Expression constantStart = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                Expression constantEnd = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                return Expression.AndAlso(Expression.LessThan(member, constantStart), Expression.GreaterThan(member, constantEnd));

            }
            if (this.MatchMode == EnumMatchMode.In)
            {
                //Type listType = typeof(List<>).MakeGenericType(member.Type);
                //MethodInfo _listAddMethod = listType.GetMethod("Add");
                //object list = Activator.CreateInstance(listType);
                //foreach (object item in this.ColumnValue as IEnumerable)
                //{
                //    _listAddMethod.Invoke(list, new[] { ChangeValueTypeToMemberType(member.Type, item) });
                //}
                //Expression constant = ConstantExpression(list);
                //MethodInfo containsMethod = _enumerableContainsMethod.MakeGenericMethod(member.Type);
                //return Expression.Call(null, containsMethod, constant, member);
                Expression orEqualsExpression = null;
                foreach (object item in this.ColumnValue as IEnumerable)
                {
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, item));
                    orEqualsExpression = orEqualsExpression.IsNull()
                        ? Expression.Equal(member, constant)
                        : Expression.OrElse(orEqualsExpression, Expression.Equal(member, constant));
                }
                return orEqualsExpression;
            }
            if (this.MatchMode == EnumMatchMode.NotIn)
            {
                //Type listType = typeof(List<>).MakeGenericType(member.Type);
                //MethodInfo _listAddMethod = listType.GetMethod("Add");
                //object list = Activator.CreateInstance(listType);
                //foreach (var item in this.ColumnValue as IEnumerable)
                //{
                //    _listAddMethod.Invoke(list, new[] { ChangeValueTypeToMemberType(member.Type, item) });
                //}
                //Expression constant = ConstantExpression(list);
                //MethodInfo containsMethod = _enumerableContainsMethod.MakeGenericMethod(member.Type);
                //return Expression.Not(Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(member.Type)), member));
                Expression andNotEqualsExpression = null;
                foreach (object item in this.ColumnValue as IEnumerable)
                {
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, item));
                    andNotEqualsExpression = andNotEqualsExpression.IsNull() ?
                        Expression.NotEqual(member, constant)
                        : Expression.AndAlso(andNotEqualsExpression, Expression.NotEqual(member, constant));
                }
                return andNotEqualsExpression;

            }
            if (this.MatchMode == EnumMatchMode.StartWith)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));

                return AttachNotNullCheckExpression(member, Expression.Call(Expression.Call(member, _objectToStringMethod), _stringStartsWithMethod, constant));

            }

            if (this.MatchMode == EnumMatchMode.EndWith)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));

                return AttachNotNullCheckExpression(member, Expression.Call(Expression.Call(member, _objectToStringMethod), _stringEndsWithMethod, constant));

            }
            return null;
        }


        private Expression AttachNotNullCheckExpression(MemberExpression nullCheckMember, Expression attachExpression)
        {
            if (typeof(ValueType).IsAssignableFrom(nullCheckMember.Type))
            {
                return attachExpression;
            }
            return Expression.AndAlso(Expression.NotEqual(nullCheckMember, Expression.Constant(null)), attachExpression);
        }



        /// <summary>
        /// 将目标对象的类型转化为预期类型
        /// </summary>
        /// <param name="memberType">目标类型</param>
        /// <param name="value">将要转化的对象</param>
        /// <returns></returns>
        protected virtual object ChangeValueTypeToMemberType(Type memberType, object value)
        {
            Type targetType = memberType;
            object returnValue = null;
            if (typeof(Enum).IsAssignableFrom(targetType))
            {
                returnValue = Enum.Parse(targetType, value?.ToString());
            }
            else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value != null)
                {
                    targetType = targetType.GenericTypeArguments[0];
                }
                returnValue = ChangeValueTypeToMemberType(targetType, (value as string) ?? value.ToString());
            }
            else
            {
                if (typeof(IConvertible).IsAssignableFrom(targetType))
                {
                    returnValue = Convert.ChangeType(value, targetType);
                }
                if (targetType.Equals(typeof(Guid)))
                {
                    returnValue = Guid.Parse((value as string) ?? value.ToString());
                }
                if (targetType.Equals(typeof(TimeSpan)))
                {
                    returnValue = TimeSpan.Parse((value as string) ?? value.ToString());
                }
            }
            return returnValue;
        }


        /// <summary>
        /// 生成常量表达式
        /// 使用属性访问表达式替代常量访问
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected virtual Expression ConstantExpression(object value)
        {
            Type targetType = value.GetType();
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (value == null)
            {
                return Expression.Constant(value, targetType);
            }

            if (targetType == typeof(string))
            {
                var constantString = Expression.Constant(new ConstantString(value));
                return Expression.Property(constantString, nameof(ConstantString.Value));
            }
            else
            {
                var expression = (Expression)Expression.Constant(value);
                return value.GetType() == targetType ? expression : Expression.Convert(expression, targetType);
            }
        }

        /// <summary>
        /// 表示文本常量
        /// </summary>
        private class ConstantString
        {
            /// <summary>
            /// 获取常量值
            /// </summary>
            public string Value { get; }

            /// <summary>
            /// 文本常量
            /// </summary>
            /// <param name="value">常量值</param>
            public ConstantString(object value)
            {
                this.Value = value?.ToString();
            }

            /// <summary>
            /// 转换为字符串
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return $@"""{this.Value}""";
            }
        }

    }









}
