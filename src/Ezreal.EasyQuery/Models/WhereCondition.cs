using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ezreal.EasyQuery.Enums;

namespace Ezreal.EasyQuery.Models
{
    public class WhereCondition
    {
        /// <summary>
        /// 搜索列
        /// </summary>
        public string Key { get; set; }

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
        public object Value { get; set; }

        /// <summary>
        /// 匹配模式
        /// </summary>
        public EnumMatchMode MatchMode { get; set; }

        //public StringComparison StringComparison { get; set; }

        private static readonly MethodInfo _objectToStringMethod =
            typeof(object).GetMethod(nameof(ToString), new Type[] { });

        private static readonly MethodInfo _stringContainsMethod =
            typeof(string).GetMethod(nameof(string.Contains), new Type[] {typeof(string)});

        private static readonly MethodInfo _stringStartsWithMethod =
            typeof(string).GetMethod(nameof(string.StartsWith), new Type[] {typeof(string)});

        private static readonly MethodInfo _stringEndsWithMethod =
            typeof(string).GetMethod(nameof(string.EndsWith), new Type[] {typeof(string)});

        private static readonly MethodInfo _stringToLowerMethod =
            typeof(string).GetMethod(nameof(string.ToLower), new Type[] { });

        private static readonly MethodInfo _enumerableContainsMethod = typeof(Enumerable).GetMethods()
            .FirstOrDefault(m =>
                m.IsGenericMethod && m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);

        public WhereCondition(string columnName, object columnValue, EnumMatchMode matchMode)
        {
            Key = columnName ?? throw new ArgumentNullException(nameof(columnName));
            Value = columnValue ?? throw new ArgumentNullException(nameof(columnValue));
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
            MemberExpression member = Expression.PropertyOrField(parameter, Key);
            if (member.Type.IsGenericType && member.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (Value != null)
                {
                    member = Expression.Property(member, "Value");
                }
            }

            switch (MatchMode)
            {
                case EnumMatchMode.Equal:
                {
                    //member==constant
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));
                    return Expression.Equal(member, constant);
                }
                case EnumMatchMode.NotEqual:
                {
                    //member!=constant
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));
                    return Expression.NotEqual(member, constant);
                }
                case EnumMatchMode.Like:
                {
                    //member.ToString().ToLower().Contains(constant.ToString().ToLower())
                    Expression constant = ConstantExpression(Value?.ToString());
                    Expression currentExpression = member.Type == typeof(string)
                        ? (Expression) member
                        : Expression.Call(member, _objectToStringMethod);
                    currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
                    currentExpression = Expression.Call(currentExpression, _stringContainsMethod,
                        Expression.Call(constant, _stringToLowerMethod));
                    return AttachNotNullCheckExpression(member, currentExpression);
                }
                case EnumMatchMode.NotLike:
                {
                    //!member.ToString().ToLower().Contains(constant.ToString().ToLower())
                    Expression constant = ConstantExpression(Value?.ToString());
                    Expression currentExpression = member.Type == typeof(string)
                        ? (Expression) member
                        : Expression.Call(member, _objectToStringMethod);
                    currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
                    currentExpression = Expression.Call(currentExpression, _stringContainsMethod,
                        Expression.Call(constant, _stringToLowerMethod));
                    currentExpression = Expression.Not(currentExpression);
                    return AttachNotNullCheckExpression(member, currentExpression);
                }
                case EnumMatchMode.Less:
                {
                    //member<constant
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));
                    return Expression.LessThan(member, constant);
                }
                case EnumMatchMode.LessOrEqual:
                {
                    //member<=constant
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));
                    return Expression.LessThanOrEqual(member, constant);
                }
                case EnumMatchMode.More:
                {
                    //member>constant
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));
                    return Expression.GreaterThan(member, constant);
                }
                case EnumMatchMode.MoreOrEqual:
                {
                    //member>=constant
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));
                    return Expression.GreaterThanOrEqual(member, constant);
                }
                case EnumMatchMode.Between:
                {
                    //member>=constant[0]&&member<=constant[1]
                    if (Value is object[] valueArray)
                    {
                        Expression constantStart = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                        Expression constantEnd = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                        return Expression.AndAlso(Expression.GreaterThanOrEqual(member, constantStart),
                            Expression.LessThanOrEqual(member, constantEnd));
                    }

                    return null;
                }
                case EnumMatchMode.NotBetween:
                {
                    //member<=constant[0]&&member>=constant[1]
                    if (Value is object[] valueArray)
                    {
                        Expression constantStart = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                        Expression constantEnd = ConstantExpression(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                        return Expression.AndAlso(Expression.LessThan(member, constantStart),
                            Expression.GreaterThan(member, constantEnd));
                    }
                    return null;
                }
                case EnumMatchMode.In:
                {
                    Expression orEqualsExpression = null;
                    if (Value == null)
                    {
                        return orEqualsExpression;
                    }

                    foreach (object item in (IEnumerable)Value)
                    {
                        Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, item));
                        orEqualsExpression = orEqualsExpression == null
                            ? Expression.Equal(member, constant)
                            : Expression.OrElse(orEqualsExpression, Expression.Equal(member, constant));
                    }

                    return orEqualsExpression;
                }
                case EnumMatchMode.NotIn:
                {
                    Expression andNotEqualsExpression = null;
                    if (Value == null)
                    {
                        return andNotEqualsExpression;
                    }

                    foreach (object item in (IEnumerable)Value)
                    {
                        Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, item));
                        andNotEqualsExpression = andNotEqualsExpression == null
                            ? Expression.NotEqual(member, constant)
                            : Expression.AndAlso(andNotEqualsExpression, Expression.NotEqual(member, constant));
                    }

                    return andNotEqualsExpression;
                }
                case EnumMatchMode.StartWith:
                {
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));

                    return AttachNotNullCheckExpression(member,
                        Expression.Call(Expression.Call(member, _objectToStringMethod), _stringStartsWithMethod,
                            constant));
                }
                case EnumMatchMode.EndWith:
                {
                    Expression constant = ConstantExpression(ChangeValueTypeToMemberType(member.Type, Value));

                    return AttachNotNullCheckExpression(member,
                        Expression.Call(Expression.Call(member, _objectToStringMethod), _stringEndsWithMethod,
                            constant));
                }
                case EnumMatchMode.All:

                default:
                    return null;
            }
        }


        private static Expression AttachNotNullCheckExpression(MemberExpression nullCheckMember,
            Expression attachExpression)
        {
            if (typeof(ValueType).IsAssignableFrom(nullCheckMember.Type))
            {
                return attachExpression;
            }

            return Expression.AndAlso(Expression.NotEqual(nullCheckMember, Expression.Constant(null)),
                attachExpression);
        }


        /// <summary>
        /// 将目标对象的类型转化为预期类型
        /// </summary>
        /// <param name="memberType">目标类型</param>
        /// <param name="value">将要转化的对象</param>
        /// <returns></returns>
        protected virtual object ChangeValueTypeToMemberType(Type memberType, object value)
        {
            if (value == null) return null;
            Type targetType = memberType;
            object returnValue = null;
            if (typeof(Enum).IsAssignableFrom(targetType))
            {
                returnValue = Enum.Parse(targetType, value?.ToString());
            }
            else if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                targetType = targetType.GenericTypeArguments[0];

                returnValue = ChangeValueTypeToMemberType(targetType, (value as string) ?? value.ToString());
            }
            else
            {
                if (typeof(IConvertible).IsAssignableFrom(targetType))
                {
                    returnValue = Convert.ChangeType(value, targetType);
                }

                if (targetType == typeof(Guid))
                {
                    returnValue = Guid.Parse((value as string) ?? value.ToString());
                }

                if (targetType == typeof(TimeSpan))
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
            if (value == null) return Expression.Constant(null);
            Type targetType = value.GetType();

            if (targetType == typeof(string))
            {
                ConstantExpression constantString = Expression.Constant(new ConstantStringWrapper(value));
                return Expression.Property(constantString, nameof(ConstantStringWrapper.Value));
            }
            else
            {
                Expression expression = (Expression) Expression.Constant(value);
                return value.GetType() == targetType ? expression : Expression.Convert(expression, targetType);
            }
        }


    }
}
