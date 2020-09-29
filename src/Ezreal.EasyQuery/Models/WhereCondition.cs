using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ezreal.EasyQuery.Enums;
using Ezreal.EasyQuery.Extensions;

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
            typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });

        private static readonly MethodInfo _stringStartsWithMethod =
            typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });

        private static readonly MethodInfo _stringEndsWithMethod =
            typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });

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
            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, Key);


            switch (MatchMode)
            {
                case EnumMatchMode.Equal:
                {
                    return GetEqualExpression(memberAccessor, Value);
                }
                case EnumMatchMode.NotEqual:
                {
                    return GetNotEqualExpression(memberAccessor, Value);
                }
                case EnumMatchMode.Like:
                {
                    return GetLikeExpression(memberAccessor, Value?.ToString());
                }
                case EnumMatchMode.NotLike:
                {
                    return GetNotLikeExpression(memberAccessor, Value?.ToString());
                }
                case EnumMatchMode.LessThan:
                {
                    return GetLessThanExpression(memberAccessor, Value);
                }
                case EnumMatchMode.LessThanOrEqual:
                {
                    return GetLessThanOrEqualExpression(memberAccessor, Value);
                }
                case EnumMatchMode.GreaterThan:
                {
                    return GetGreaterThanExpression(memberAccessor, Value);
                }
                case EnumMatchMode.GreaterThanOrEqual:
                {
                    return GetGreaterThanOrEqualExpression(memberAccessor, Value);
                }
                case EnumMatchMode.Between:
                {
                    return GetBetweenExpression(memberAccessor, Value);
                }
                case EnumMatchMode.NotBetween:
                {
                    return GetNotBetweenExpression(memberAccessor, Value);
                }
                case EnumMatchMode.In:
                {
                    return GetInExpression(memberAccessor, Value);
                }
                case EnumMatchMode.NotIn:
                {
                    return GetNotInExpression(memberAccessor, Value);
                }
                case EnumMatchMode.StartWith:
                {
                    return GetStartWithExpression(memberAccessor, Value?.ToString());
                }
                case EnumMatchMode.EndWith:
                {
                    return GetEndWithExpression(memberAccessor, Value?.ToString());
                }
                case EnumMatchMode.All:

                default:
                    return null;
            }
        }




        /// <summary>
        /// memberAccessor==value
        /// </summary>
        /// <param name="memberAccessor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Expression GetEqualExpression(MemberExpression memberAccessor, object value)
        {
            Expression constant = ConstantExpression(ChangeValueTypeToMemberType(memberAccessor.Type, value));

            MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
            if (nullableStructValueAccessor == null || (nullableStructValueAccessor != null && value == null))
            {
                return Expression.Equal(memberAccessor, constant);
            }
            else
            {
                return Expression.Equal(nullableStructValueAccessor, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
            }
        }
        /// <summary>
        /// memberAccessor!=value
        /// </summary>
        /// <param name="memberAccessor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Expression GetNotEqualExpression(MemberExpression memberAccessor, object value)
        {
            return Expression.Not(GetEqualExpression(memberAccessor, value));
        }
        /// <summary>
        /// <para>
        /// [ValueType]------>member.ToString().ToLower().Contains(constant.ToLower())
        /// </para>
        /// <para>
        /// [NullableType]------>member!=null AndAlso member.ToString().ToLower().Contains(constant.ToLower())
        /// </para>
        /// <para>
        /// [value is null or empty]------>null
        /// </para>
        /// </summary>
        /// <param name="memberAccessor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Expression GetLikeExpression(MemberExpression memberAccessor, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"“{nameof(value)}”不能是 Null 或为空。", nameof(value));
            }

            Expression constant = ConstantExpression(value);
            Expression currentExpression = memberAccessor.Type == typeof(string)
                ? (Expression)memberAccessor
                : Expression.Call(memberAccessor, _objectToStringMethod);
            currentExpression = Expression.Call(currentExpression, _stringToLowerMethod);
            currentExpression = Expression.Call(currentExpression, _stringContainsMethod,
                Expression.Call(constant, _stringToLowerMethod));
            return currentExpression.AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
        }
        /// <summary>
        /// Expression.Not(<see cref="GetLikeExpression"/>)
        /// </summary>
        /// <param name="memberAccessor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual Expression GetNotLikeExpression(MemberExpression memberAccessor, string value)
        {


            return Expression.Not(GetLikeExpression(memberAccessor, value));
        }

        protected virtual Expression GetLessThanExpression(MemberExpression memberAccessor, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            //member<constant
            Expression constant = ConstantExpression(ChangeValueTypeToMemberType(memberAccessor.Type, value));

            MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
            if (nullableStructValueAccessor == null)
            {
                return Expression.LessThan(memberAccessor, constant);
            }
            else
            {
                return Expression.LessThan(nullableStructValueAccessor, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
            }

        }

        protected virtual Expression GetLessThanOrEqualExpression(MemberExpression memberAccessor, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            //member<=constant
            Expression constant = ConstantExpression(ChangeValueTypeToMemberType(memberAccessor.Type, value));
            MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
            if (nullableStructValueAccessor == null)
            {
                return Expression.LessThanOrEqual(memberAccessor, constant);
            }
            else
            {
                return Expression.LessThanOrEqual(nullableStructValueAccessor, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
            }
        }

        protected virtual Expression GetGreaterThanExpression(MemberExpression memberAccessor, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            //member>constant
            Expression constant = ConstantExpression(ChangeValueTypeToMemberType(memberAccessor.Type, value));
            MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
            if (nullableStructValueAccessor == null)
            {
                return Expression.GreaterThan(memberAccessor, constant);
            }
            else
            {
                return Expression.GreaterThan(nullableStructValueAccessor, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
            }
        }
        protected virtual Expression GetGreaterThanOrEqualExpression(MemberExpression memberAccessor, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            //member>=constant
            Expression constant = ConstantExpression(ChangeValueTypeToMemberType(memberAccessor.Type, value));
            MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
            if (nullableStructValueAccessor == null)
            {
                return Expression.GreaterThanOrEqual(memberAccessor, constant);
            }
            else
            {
                return Expression.GreaterThanOrEqual(nullableStructValueAccessor, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
            }
        }


        protected virtual Expression GetBetweenExpression(MemberExpression memberAccessor, object value)
        {

            //member>=constant[0]&&member<=constant[1]
            if (value is object[] valueArray)
            {

                MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
                MemberExpression currentMemberAccessor = nullableStructValueAccessor != null ? nullableStructValueAccessor : memberAccessor;
                Expression constantStart = ConstantExpression(ChangeValueTypeToMemberType(currentMemberAccessor.Type, valueArray[0]));
                Expression constantEnd = ConstantExpression(ChangeValueTypeToMemberType(currentMemberAccessor.Type, valueArray[1]));
                Expression result = Expression.AndAlso(
                        Expression.GreaterThanOrEqual(currentMemberAccessor, constantStart),
                        Expression.LessThanOrEqual(currentMemberAccessor, constantEnd)
                        );
                if (nullableStructValueAccessor != null)
                {
                    result = result.AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
                }
                return result;
            }
            else
            {
                throw new ArgumentException(nameof(value) + " must be array which length==2", nameof(value));
            }

        }


        protected virtual Expression GetNotBetweenExpression(MemberExpression memberAccessor, object value)
        {
            return Expression.Not(GetBetweenExpression(memberAccessor, value));
        }

        protected virtual Expression GetInExpression(MemberExpression memberAccessor, object value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Expression result = null;
            MemberExpression nullableStructValueAccessor = memberAccessor.GetNullableStructValueAccessExpression();
            MemberExpression currentMemberAccessor = nullableStructValueAccessor == null ? memberAccessor : nullableStructValueAccessor;
            foreach (object item in (IEnumerable)value)
            {
                Expression constant = ConstantExpression(ChangeValueTypeToMemberType(memberAccessor.Type, item));
                result = result == null
                    ? Expression.Equal(currentMemberAccessor, constant)
                    : Expression.OrElse(result, Expression.Equal(currentMemberAccessor, constant));
            }
            if (nullableStructValueAccessor != null)
            {
                result = result.AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
            }
            return result;
        }
        protected virtual Expression GetNotInExpression(MemberExpression memberAccessor, object value)
        {
            return Expression.Not(GetInExpression(memberAccessor, value));
        }

        protected virtual Expression GetStartWithExpression(MemberExpression memberAccessor, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"“{nameof(value)}”不能是 Null 或为空。", nameof(value));
            }

            Expression constant = ConstantExpression(value);
            Expression caller = memberAccessor.Type == typeof(string) ? (Expression)memberAccessor : Expression.Call(memberAccessor, _objectToStringMethod);

            return
                Expression.Call(caller, _stringStartsWithMethod, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
        }


        protected virtual Expression GetEndWithExpression(MemberExpression memberAccessor, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"“{nameof(value)}”不能是 Null 或为空。", nameof(value));
            }

            Expression constant = ConstantExpression(value);
            Expression caller = memberAccessor.Type == typeof(string) ? (Expression)memberAccessor : Expression.Call(memberAccessor, _objectToStringMethod);

            return
                Expression.Call(caller, _stringEndsWithMethod, constant).AttachAndAlsoMemberNotNullCheckExpressionOnLeft(memberAccessor);
        }



















        /// <summary>
        /// 生成常量表达式
        /// 当传入值是字符串时,使用<see cref="ConstantStringWrapper"/>包装替代常量访问
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
                Expression expression = Expression.Constant(value);
                return value.GetType() == targetType ? expression : Expression.Convert(expression, targetType);
            }
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

                returnValue = ChangeValueTypeToMemberType(targetType, value);
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





    }
}
