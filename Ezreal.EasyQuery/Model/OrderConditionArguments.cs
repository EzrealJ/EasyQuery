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

        protected List<OrderConditionFilterAttribute> _orderConditionAttributeList;
        public virtual void InvokeParameterFilter(IEnumerable<OrderConditionFilterAttribute> orderConditionAttributes)
        {

            _orderConditionAttributeList = orderConditionAttributes.Where(opa => !opa.ColumnName.IsNullOrNoItems()).ToList();

        }

        public virtual void SetParameter(OrderCondition orderCondition)
        {
            if (!_orderConditionAttributeList.IsNullOrNoItems())
            {
                bool allowOrder = _orderConditionAttributeList.Exists(opa => opa.ColumnName.Contains(orderCondition.ColumnName) && (opa.Pattern & orderCondition.Pattern) == opa.Pattern);
                if (!allowOrder)
                {
                    return;
                }
            }

            this.Add(orderCondition);
        }

        public virtual void InitializeFromJsonObjectString(string jsonArrayString)
        {
            if (jsonArrayString[0] != '[')
            {
                jsonArrayString = $"[{jsonArrayString}]";
            }
            List<OrderCondition> orderConditionList = JsonConvert.DeserializeObject<List<OrderCondition>>(jsonArrayString);
            Initialize(orderConditionList);
        }

        protected virtual void Initialize(List<OrderCondition> orderConditionList)
        {
            this.AddRange(orderConditionList);
        }

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
                    queryable = order.Pattern == OrderCondition.enumOrderPattern.Asc ?
                    (queryable as IOrderedQueryable<TDBOSource>).ThenBy(Expression.Lambda<Func<TDBOSource, dynamic>>(member, parameter))
                    :
                    (queryable as IOrderedQueryable<TDBOSource>).ThenByDescending(Expression.Lambda<Func<TDBOSource, dynamic>>(member, new[] { parameter }));
                }
                else
                {
                    queryable = order.Pattern == OrderCondition.enumOrderPattern.Asc ?
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
