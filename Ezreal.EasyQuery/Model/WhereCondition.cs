using Ezreal.EasyQuery.Enums;
using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
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
        /// 搜索值
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
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.Equal(member, constant);
            }
            if (this.MatchMode == EnumMatchMode.NotEqual)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.NotEqual(member, constant);
            }
            if (this.MatchMode == EnumMatchMode.Like)
            {
                ConstantExpression constant = Expression.Constant(this.ColumnValue.ToString());
                Expression currentExpression = member.Type == typeof(string) ? (Expression)member : Expression.Call(member, _objectToStringMethod);
                currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
                currentExpression = Expression.Call(currentExpression, _stringContainsMethod, Expression.Call(constant, _stringToLowerMethod));
                return AttachNotNullCheckExpression(member, currentExpression);

            }
            if (this.MatchMode == EnumMatchMode.NotLike)
            {
                ConstantExpression constant = Expression.Constant(this.ColumnValue.ToString());
                Expression currentExpression = member.Type == typeof(string) ? (Expression)member : Expression.Call(member, _objectToStringMethod);
                currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
                currentExpression = Expression.Call(currentExpression, _stringContainsMethod, Expression.Call(constant, _stringToLowerMethod));
                currentExpression = Expression.Not(currentExpression);
                return AttachNotNullCheckExpression(member, currentExpression);

            }
            if (this.MatchMode == EnumMatchMode.Less)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.LessThan(member, constant);

            }
            if (this.MatchMode == EnumMatchMode.LessOrEqual)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.LessThanOrEqual(member, constant);
            }
            if (this.MatchMode == EnumMatchMode.More)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.GreaterThan(member, constant);

            }
            if (this.MatchMode == EnumMatchMode.MoreOrEqual)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));
                return Expression.GreaterThanOrEqual(member, constant);
            }


            if (this.MatchMode == EnumMatchMode.Between)
            {

                object[] valueArray = this.ColumnValue as object[];
                ConstantExpression constantStart = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                ConstantExpression constantEnd = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                return Expression.And(Expression.GreaterThanOrEqual(member, constantStart), Expression.LessThanOrEqual(member, constantEnd));

            }
            if (this.MatchMode == EnumMatchMode.NotBetween)
            {                
                object[] valueArray = this.ColumnValue as object[];
                ConstantExpression constantStart = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                ConstantExpression constantEnd = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                return Expression.And(Expression.LessThan(member, constantStart), Expression.GreaterThan(member, constantEnd));

            }
            if (this.MatchMode == EnumMatchMode.In)
            {
                Type listType = typeof(List<>).MakeGenericType(member.Type);
                MethodInfo _listAddMethod = listType.GetMethod("Add");
                object list = Activator.CreateInstance(listType);
                foreach (var item in this.ColumnValue as Array)
                {
                    _listAddMethod.Invoke(list, new[] { ChangeValueTypeToMemberType(member.Type, item) });
                }
                ConstantExpression constant = Expression.Constant(list);
                MethodInfo containsMethod = _enumerableContainsMethod.MakeGenericMethod(member.Type);
                return Expression.Call(null, containsMethod, constant, member);

            }
            if (this.MatchMode == EnumMatchMode.NotIn)
            {
                Type listType = typeof(List<>).MakeGenericType(member.Type);
                MethodInfo _listAddMethod = listType.GetMethod("Add");
                object list = Activator.CreateInstance(listType);
                foreach (var item in this.ColumnValue as Array)
                {
                    _listAddMethod.Invoke(list, new[] { ChangeValueTypeToMemberType(member.Type, item) });
                }
                ConstantExpression constant = Expression.Constant(list);
                MethodInfo containsMethod = _enumerableContainsMethod.MakeGenericMethod(member.Type);
                return Expression.Not(Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(member.Type)), member));

            }
            if (this.MatchMode == EnumMatchMode.StartWith)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));

                return AttachNotNullCheckExpression(member, Expression.Call(Expression.Call(member, _objectToStringMethod), _stringStartsWithMethod, constant));

            }

            if (this.MatchMode == EnumMatchMode.EndWith)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));

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
                else
                {
                    returnValue = value;
                }
            }
            else

            //if (!(targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
            {
                if (typeof(IConvertible).IsAssignableFrom(targetType))
                {
                    returnValue = Convert.ChangeType(value, targetType);
                }
                if (targetType.Equals(typeof(Guid)))
                {
                    returnValue = Guid.Parse(value.ToString());
                }
            }
            return returnValue;
        }

    }









}
