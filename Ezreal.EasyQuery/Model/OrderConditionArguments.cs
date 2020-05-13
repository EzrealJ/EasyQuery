using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Enums;
using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class OrderConditionArguments : List<OrderCondition>
    {
        public static readonly MethodInfo[] _queryableMethods = typeof(Queryable).GetMethods();
        private static readonly MethodInfo _orderByMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2);
        private static readonly MethodInfo _orderByDescendingMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.OrderByDescending) && m.GetParameters().Length == 2);
        private static readonly MethodInfo _thenByMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.ThenBy) && m.GetParameters().Length == 2);
        private static readonly MethodInfo _thenByDescendingMethod = _queryableMethods.FirstOrDefault(m => m.IsGenericMethod && m.Name == nameof(Queryable.ThenByDescending) && m.GetParameters().Length == 2);
        public virtual IQueryable<TDBOSource> GetOrderedQueryable<TDBOSource>(IQueryable<TDBOSource> queryable)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }
            ParameterExpression parameter = Expression.Parameter(typeof(TDBOSource), "s");
         
            this.ForEach(order =>
            {
                MemberExpression member = Expression.PropertyOrField(parameter, order.ColumnName);
                Type funcType = typeof(Func<,>).MakeGenericType(typeof(TDBOSource), member.Type);
                string queryableExpressionString = queryable.Expression.ToString();
                if (queryableExpressionString.Contains(nameof(Queryable.OrderBy)) || queryableExpressionString.Contains(nameof(Queryable.OrderByDescending)))
                {
                    IOrderedQueryable<TDBOSource> orderedQueryable = queryable as IOrderedQueryable<TDBOSource>;
                    if (order.OrderMode == EnumOrderMode.Asc)
                    {
                        MethodInfo method = _thenByMethod.MakeGenericMethod(typeof(TDBOSource), member.Type);
                        queryable = method.Invoke(null, new object[] { orderedQueryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TDBOSource>;
                    }
                    else
                    {
                        MethodInfo method = _thenByDescendingMethod.MakeGenericMethod(typeof(TDBOSource), member.Type);
                        queryable = method.Invoke(null, new object[] { orderedQueryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TDBOSource>;
                    }

                }
                else
                {
                    if (order.OrderMode == EnumOrderMode.Asc)
                    {
                        MethodInfo method = _orderByMethod.MakeGenericMethod(typeof(TDBOSource), member.Type);
                        queryable = method.Invoke(null, new object[] { queryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TDBOSource>;
                    }
                    else
                    {
                        MethodInfo method = _orderByDescendingMethod.MakeGenericMethod(typeof(TDBOSource), member.Type);
                        queryable = method.Invoke(null, new object[] { queryable, Expression.Lambda(funcType, member, parameter) }) as IOrderedQueryable<TDBOSource>;
                    }                
                }
            });

            return queryable;
        }

    }
}
