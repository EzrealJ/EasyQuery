using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Enums;
using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class OrderConditionArguments : List<OrderCondition>
    {
        public virtual IQueryable<TDBOSource> GetOrderedQueryable<TDBOSource>(IQueryable<TDBOSource> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }
            ParameterExpression parameter = Expression.Parameter(typeof(TDBOSource), "s");
            bool IsOrdered = false;
            this.ForEach(order =>
            {
                MemberExpression member = Expression.PropertyOrField(parameter, order.ColumnName);

                string queryableExpressionString = queryable.Expression.ToString();
                if (IsOrdered && (queryableExpressionString.Contains(nameof(Queryable.OrderBy)) || queryableExpressionString.Contains(nameof(Queryable.OrderByDescending))))
                {
                    queryable = order.OrderMode == EnumOrderMode.Asc ?
                    (queryable as IOrderedQueryable<TDBOSource>).ThenBy(Expression.Lambda<Func<TDBOSource, dynamic>>(member, parameter))
                    :
                    (queryable as IOrderedQueryable<TDBOSource>).ThenByDescending(Expression.Lambda<Func<TDBOSource, dynamic>>(member, new[] { parameter }));
                }
                else
                {
                    queryable = order.OrderMode == EnumOrderMode.Asc ?
                        (queryable).OrderBy(Expression.Lambda<Func<TDBOSource, dynamic>>(member, parameter))
                        :
                        (queryable).OrderByDescending(Expression.Lambda<Func<TDBOSource, dynamic>>(member, new[] { parameter }));
                    IsOrdered = true;
                }
            });

            return queryable;
        }

    }
}
