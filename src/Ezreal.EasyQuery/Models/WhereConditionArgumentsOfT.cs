using System;
using System.Linq.Expressions;

namespace Ezreal.EasyQuery.Models
{
    public class WhereConditionArguments<TSource> : WhereConditionArguments
    {
        /// <summary>
        /// 获取Lambda表达式
        /// </summary>
        /// <returns></returns>
        public virtual Expression<Func<TSource, bool>> GetWhereLambdaExpression() =>
            GetWhereLambdaExpression<TSource>();

        public virtual Expression<Func<TDBOSource, bool>> GetWhereLambdaExpression<TDBOSource>()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TDBOSource), "t");
            Expression where = GetExpression<TDBOSource>(parameter);
            if (where == null) return null;
            Expression<Func<TDBOSource, bool>> expression = Expression.Lambda<Func<TDBOSource, bool>>(where, parameter);
            return expression;
        }
    }
}
