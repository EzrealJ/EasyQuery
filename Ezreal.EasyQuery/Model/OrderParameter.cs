using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ezreal.EasyQuery.Model
{
    public class OrderParameter
    {
        /// <summary>
        /// 排序列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 排序模式["asc"/"desc"]
        /// </summary>
        public enumOrderPattern Pattern { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        [Flags]
        public enum enumOrderPattern
        {
            Asc = 1,
            Desc = 2,
            All = 3,
        }

    }

    public class OrderParameterArguments : List<OrderParameter>
    {

        protected List<OrderParameterAttribute> _orderParameterAttributeList;
        public virtual void InvokeParameterFilter(IEnumerable<OrderParameterAttribute> orderParameterAttributes)
        {
            if (_orderParameterAttributeList.IsNullOrNoItems())
            {
                _orderParameterAttributeList = orderParameterAttributes.Where(opa => !opa.ColumnName.IsNullOrNoItems()).ToList();
            }
        }

        public virtual void SetParameter(OrderParameter orderParameter)
        {
            bool allowOrder = _orderParameterAttributeList.Exists(opa =>
            opa.ColumnName.Contains(orderParameter.ColumnName)
            && (opa.Pattern & orderParameter.Pattern) == opa.Pattern);
            if (!allowOrder) return;
            this.Add(orderParameter);
        }

        public virtual void InitializeFromJsonObjectString(string jsonArrayString)
        {
            if (jsonArrayString[0] != '[')
            {
                jsonArrayString = $"[{jsonArrayString}]";
            }
            List<OrderParameter> orderParameterList = JsonConvert.DeserializeObject<List<OrderParameter>>(jsonArrayString);
            Initialize(orderParameterList);
        }

        protected virtual void Initialize(List<OrderParameter> orderParameterList)
        {
            this.AddRange(orderParameterList);
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
                    queryable = order.Pattern == OrderParameter.enumOrderPattern.Asc ?
                    (queryable as IOrderedQueryable<TDBOSource>).ThenBy(Expression.Lambda<Func<TDBOSource, dynamic>>(member, parameter))
                    :
                    (queryable as IOrderedQueryable<TDBOSource>).ThenByDescending(Expression.Lambda<Func<TDBOSource, dynamic>>(member, new[] { parameter }));
                }
                else
                {                    
                    queryable = order.Pattern == OrderParameter.enumOrderPattern.Asc ?
                        (queryable).OrderBy(Expression.Lambda<Func<TDBOSource, dynamic>>(member, parameter))
                        :
                        (queryable).OrderByDescending(Expression.Lambda<Func<TDBOSource, dynamic>>(member, new[] { parameter }));
                    IsOrdered = true;
                }
            });

            return queryable;
        }

    }

    public class OrderParameterArguments<TSource> : OrderParameterArguments
    {
        protected List<PropertyInfo> _sourcePropertyInfos = typeof(TSource).GetProperties().ToList();
        public override void SetParameter(OrderParameter orderParameter)
        {
            if (!_sourcePropertyInfos.Exists(spi => spi.Name.Equals(orderParameter.ColumnName))) return;
            base.SetParameter(orderParameter);
        }

        protected override void Initialize(List<OrderParameter> orderParameterList)
        {
            orderParameterList = orderParameterList.Where(op => _sourcePropertyInfos.Exists(spi => spi.Name.Equals(op.ColumnName))).ToList();
            base.Initialize(orderParameterList);
        }


    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class OrderParameterAttribute : System.Attribute
    {
        public OrderParameterAttribute(OrderParameter.enumOrderPattern pattern, params string[] columnName)
        {
            this.Pattern = pattern;
            this.ColumnName = columnName;
        }

        public OrderParameter.enumOrderPattern Pattern { get; }
        public string[] ColumnName { get; }
    }
}
