using Ezreal.Extension.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class WhereConditionArguments<TSource> : WhereConditionArguments, IMultilevelArguments<TSource>
    {
        List<System.Reflection.PropertyInfo> _sourcePropertyInfos = typeof(TSource).GetProperties().ToList();
        private static readonly MethodInfo _objectToStringMethod = typeof(object).GetMethod(nameof(object.ToString), new Type[] { });
        private static readonly MethodInfo _stringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });
        public override void InvokeParameterFilter(IEnumerable<WhereConditionFilterAttribute> whereParameterAttributes)
        {
            if (_whereParameterAttributeList.IsNullOrNoItems())
            {
                _whereParameterAttributeList = _whereParameterAttributeList ?? new List<WhereConditionFilterAttribute>();
                whereParameterAttributes.ToList().ForEach(wpa =>
                {

                    wpa.ColumnName = wpa.ColumnName.Where(cName => _sourcePropertyInfos.Exists(p => p.Name.Equals(cName)));
                    _whereParameterAttributeList.Add(wpa);
                });
            }
        }
        protected override List<WhereCondition> InvokeCriteriaFilter(List<WhereCondition> arguments, enumWherePattern wherePattern)
        {
            arguments = base.InvokeCriteriaFilter(arguments, wherePattern);
            arguments?.ForEach(arg =>
            {
                List<WhereConditionFilterAttribute> whereParameterAttributeList = _whereParameterAttributeList.Where(
                    attr =>

                    (
                    (attr.ColumnName.IsNullOrNoItems() && _sourcePropertyInfos.Exists(p => p.Name?.ToLower() == arg.ColumnName?.ToLower()))
                    ||
                    (!attr.ColumnName.IsNullOrNoItems() && attr.ColumnName.Contains(arg.ColumnName.Trim()))
                    )

                    && (attr.WherePattern & wherePattern) == wherePattern)?.ToList();
                Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(arg.ColumnName))?.PropertyType;
                if (whereParameterAttributeList.IsNullOrNoItems()) return;
                if (targetType.IsNull()) return;
                targetType = targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>) ? targetType.GenericTypeArguments[0] : targetType;
                if ((wherePattern & (enumWherePattern.Between | enumWherePattern.NotBetween | enumWherePattern.In | enumWherePattern.NotIn)) == wherePattern)
                {
                    object[] targetArray = Array.ConvertAll(((string)arg.ColumnValue)?.Trim().Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries), t => Convert.ChangeType(t, targetType));

                    if (targetArray.IsNullOrNoItems()
                    || ((wherePattern & (enumWherePattern.In | enumWherePattern.NotIn)) == wherePattern && targetArray.Length != 2))
                    {
                        arguments.Remove(arg);
                    }
                    arg.ColumnValue = targetArray;
                }
                else if ((wherePattern & enumWherePattern.Like) == enumWherePattern.Like)
                {
                    //No conversion required
                }
                else
                {
                    arg.ColumnValue = Convert.ChangeType(arg.ColumnValue, targetType);
                }
            });
            return arguments;
        }
        public virtual Expression<Func<TSource, bool>> GetWhereLambdaExpression() => this.GetWhereLambdaExpression<TSource>();
        public virtual Expression<Func<TDBOSource, bool>> GetWhereLambdaExpression<TDBOSource>()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TDBOSource), "t");

            Expression where = GetConditionExpression(parameter);
            Expression<Func<TDBOSource, bool>> expression = Expression.Lambda<Func<TDBOSource, bool>>(where, new[] { parameter });
            return expression;
        }

        public Expression GetConditionExpression(ParameterExpression parameter)
        {
            Expression where = Expression.Equal(Expression.Constant(1), Expression.Constant(1));
            //Expression where = Expression.Constant(true);
            try
            {

                this.EqualArguments?.ForEach(ea =>
                {

                    MemberExpression member = Expression.PropertyOrField(parameter, ea.ColumnName);

                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, ea.ColumnValue));
                    Expression currentExpression = Expression.Equal(member, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.NotEqualArguments?.ForEach(nea =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nea.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, nea.ColumnValue));
                    Expression currentExpression = Expression.NotEqual(member, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });


                this.LikeArguments?.ForEach(la =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, la.ColumnName);
                    ConstantExpression constant = Expression.Constant(la.ColumnValue.ToString());
                    Expression currentExpression = Expression.Call(Expression.Call(member, _objectToStringMethod), _stringContainsMethod, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.NotLikeArguments?.ForEach(nla =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nla.ColumnName);
                    ConstantExpression constant = Expression.Constant(nla.ColumnValue.ToString());
                    Expression currentExpression = Expression.Not(Expression.Call(Expression.Call(member, _objectToStringMethod), _stringContainsMethod, constant));
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.LessArguments?.ForEach(la =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, la.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, la.ColumnValue));
                    Expression currentExpression = Expression.LessThan(member, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.LessOrEqualArguments?.ForEach(loea =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, loea.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, loea.ColumnValue));
                    Expression currentExpression = Expression.LessThanOrEqual(member, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.MoreArguments?.ForEach(ma =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, ma.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, ma.ColumnValue));
                    Expression currentExpression = Expression.GreaterThan(member, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.MoreOrEqualArguments?.ForEach(moea =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, moea.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, moea.ColumnValue));
                    Expression currentExpression = Expression.GreaterThanOrEqual(member, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.BetweenArguments?.ForEach(ba =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, ba.ColumnName);
                    Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(ba.ColumnName))?.PropertyType;
                    object[] valueArray = ba.ColumnValue as object[];
                    ConstantExpression constantStart = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                    ConstantExpression constantEnd = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                    Expression currentExpression = Expression.And(Expression.GreaterThanOrEqual(member, constantStart), Expression.LessThanOrEqual(member, constantEnd));
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.NotBetweenArguments?.ForEach(nba =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nba.ColumnName);
                    Type targetType = _sourcePropertyInfos.FirstOrDefault(porp => porp.Name.Equals(nba.ColumnName))?.PropertyType;
                    object[] valueArray = nba.ColumnValue as object[];
                    ConstantExpression constantStart = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[0]));
                    ConstantExpression constantEnd = Expression.Constant(ChangeValueTypeToMemberType(member.Type, valueArray[1]));
                    Expression currentExpression = Expression.And(Expression.LessThan(member, constantStart), Expression.GreaterThan(member, constantEnd));
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.InArguments?.ForEach(ia =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, ia.ColumnName);
                    ConstantExpression constant = Expression.Constant(ia.ColumnValue);
                    Type targetType = member.Type;
                    MethodInfo containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(targetType);
                    Expression currentExpression = Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(targetType)), member);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.NotInArguments?.ForEach(nia =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, nia.ColumnName);
                    ConstantExpression constant = Expression.Constant(nia.ColumnValue);
                    Type targetType = member.Type;
                    MethodInfo containsMethod = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.IsGenericMethod && m.Name == "Contains" && m.GetParameters().Length == 2).MakeGenericMethod(targetType);
                    Expression currentExpression = Expression.Not(Expression.Call(null, containsMethod, Expression.TypeAs(constant, typeof(IEnumerable<>).MakeGenericType(targetType)), member));
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.StartWithArguments?.ForEach(swa =>
                {
                    MemberExpression member = Expression.PropertyOrField(parameter, swa.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, swa.ColumnValue));
                    var startsWithMethod = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                    Expression currentExpression = Expression.Call(Expression.Call(member, _objectToStringMethod), startsWithMethod, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
                this.EndWithArguments?.ForEach(ewa =>
                {

                    MemberExpression member = Expression.PropertyOrField(parameter, ewa.ColumnName);
                    ConstantExpression constant = Expression.Constant(ChangeValueTypeToMemberType(member.Type, ewa.ColumnValue));
                    var endsWithMethod = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });
                    Expression currentExpression = Expression.Call(Expression.Call(member, _objectToStringMethod), endsWithMethod, constant);
                    where = Expression.AndAlso(where, currentExpression);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return where;
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
