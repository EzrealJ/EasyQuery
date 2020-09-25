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
            this.GetWhereLambdaExpression<TSource>();

        public virtual Expression<Func<TDBOSource, bool>> GetWhereLambdaExpression<TDBOSource>()
        {
            var parameter = Expression.Parameter(typeof(TDBOSource), "t");
            var where = GetExpression<TDBOSource>(parameter);
            if (where == null) return null;
            var expression = Expression.Lambda<Func<TDBOSource, bool>>(where, parameter);
            return expression;
        }
    }
}