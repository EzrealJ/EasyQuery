using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class MultilevelWhereConditionArguments<TSource> : MultilevelWhereConditionArguments, IMultilevelArguments<TSource>
    {
        /// <summary>
        /// 内联参数,将采用Or进行连接
        /// </summary>
        public virtual new List<IMultilevelArguments<TSource>> InternalMultilevelArguments { get; set; }



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
            Expression where = Expression.Equal(Expression.Constant(1), Expression.Constant(0));
            //Expression where = Expression.Constant(false);
            foreach (var item in InternalMultilevelArguments)
            {
                if (item is WhereConditionArguments<TSource> whereCondition)
                {
                    where = Expression.OrElse(where, whereCondition.GetConditionExpression(parameter));
                }
                if (item is MultilevelWhereConditionArguments<TSource> multilevelWhereCondition)
                {
                    where = Expression.OrElse(where, multilevelWhereCondition.GetConditionExpression(parameter));
                }
            }
            return where;
        }
    }
}
