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


    public class WhereCondition : IMultilevelable
    {
        /// <summary>
        /// 搜索列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 搜索值
        /// </summary>
        public object ColumnValue { get; set; }

        public EnumMatchMode MatchMode { get; set; }

        private static readonly MethodInfo _objectToStringMethod = typeof(object).GetMethod(nameof(object.ToString), new Type[] { });
        private static readonly MethodInfo _stringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
        private static readonly MethodInfo _stringStartsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
        private static readonly MethodInfo _stringEndsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
        /// <summary>
        /// 获取表达式
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Expression GetExpression<TSource>(ParameterExpression parameter)
        {
            MemberExpression member = Expression.PropertyOrField(parameter, this.ColumnName);
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
                return Expression.Call(Expression.Call(member, _objectToStringMethod), _stringContainsMethod, constant);

            }
            if (this.MatchMode == EnumMatchMode.NotLike)
            {
                ConstantExpression constant = Expression.Constant(this.ColumnValue.ToString());
                return Expression.Not(Expression.Call(Expression.Call(member, _objectToStringMethod), _stringContainsMethod, constant));

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
                ConstantExpression constant = Expression.Constant(this.ColumnValue);
                Type targetType = member.Type;
                MethodInfo containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(targetType);
                return Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(targetType)), member);

            }
            if (this.MatchMode == EnumMatchMode.NotIn)
            {
                ConstantExpression constant = Expression.Constant(this.ColumnValue);
                Type targetType = member.Type;
                MethodInfo containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(targetType);
                return Expression.Not(Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(targetType)), member));

            }
            if (this.MatchMode == EnumMatchMode.StartWith)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));

                return Expression.Call(Expression.Call(member, _objectToStringMethod), _stringStartsWithMethod, constant);

            }

            if (this.MatchMode == EnumMatchMode.EndWith)
            {
                ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, this.ColumnValue));

                return Expression.Call(Expression.Call(member, _objectToStringMethod), _stringEndsWithMethod, constant);

            }
            return null;
        }


        /// <summary>
        /// 将值类型转化为本地类型
        /// </summary>
        /// <param name="memberType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual object ChangeValueTypeToMemberType(Type memberType, object value)
        {
            Type targetType = memberType;
            object returnValue = null;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
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
            if (!(targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>))))
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
