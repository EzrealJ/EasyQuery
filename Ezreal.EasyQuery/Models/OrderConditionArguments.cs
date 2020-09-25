using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ezreal.EasyQuery.Enums;

namespace Ezreal.EasyQuery.Models
{
    public class OrderConditionArguments : List<OrderCondition>
    {
        private static readonly MethodInfo[] _queryableMethods = typeof(Queryable).GetMethods();
        private static readonly MethodInfo _orderByMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);
        private static readonly MethodInfo _orderByDescendingMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);
        private static readonly MethodInfo _thenByMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.ThenBy) && m.GetParameters().Length == 2);
        private static readonly MethodInfo _thenByDescendingMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.ThenByDescending) && m.GetParameters().Length == 2);
        public virtual IQueryable<TSource> GetOrderedQueryable<TSource>(IQueryable<TSource> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }
            var parameter = Expression.Parameter(typeof(TSource), "s");
            var queryableExpressionString = queryable.Expression.ToString();
            this.ForEach(order =>
            {
                var member = Expression.PropertyOrField(parameter, order.ColumnName);
                var funcType = typeof(Func<,>).MakeGenericType(typeof(TSource), member.Type);
                if (queryableExpressionString.Contains(nameof(Queryable.OrderBy)) || queryableExpressionString.Contains(nameof(Queryable.OrderByDescending)))
                {
                    var orderedQueryable = queryable as IOrderedQueryable<TSource>;
                    if (order.OrderMode == EnumOrderMode.Asc)
                    {
                        var method = _thenByMethod.MakeGenericMethod(typeof(TSource), member.Type);
                        queryable = method.Invoke(null, new object[] { orderedQueryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TSource>;
                    }
                    else
                    {
                        var method = _thenByDescendingMethod.MakeGenericMethod(typeof(TSource), member.Type);
                        queryable = method.Invoke(null, new object[] { orderedQueryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TSource>;
                    }

                }
                else
                {
                    if (order.OrderMode == EnumOrderMode.Asc)
                    {
                        var method = _orderByMethod.MakeGenericMethod(typeof(TSource), member.Type);
                        queryable = method.Invoke(null, new object[] { queryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TSource>;
                    }
                    else
                    {
                        var method = _orderByDescendingMethod.MakeGenericMethod(typeof(TSource), member.Type);
                        queryable = method.Invoke(null, new object[] { queryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TSource>;
                    }                
                }
            });

            return queryable;
        }

    }
}
