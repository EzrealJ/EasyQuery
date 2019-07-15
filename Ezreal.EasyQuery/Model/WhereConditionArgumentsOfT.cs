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
        public virtual Expression<Func<TSource, bool>> GetWhereLambdaExpression() => this.GetWhereLambdaExpression<TSource>();
        public virtual Expression<Func<TDBOSource, bool>> GetWhereLambdaExpression<TDBOSource>()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TDBOSource), "t");

            Expression where = GetExpression<TDBOSource>(parameter);
            Expression<Func<TDBOSource, bool>> expression = Expression.Lambda<Func<TDBOSource, bool>>(where, new[] { parameter });
            return expression;
        }






    }
}
