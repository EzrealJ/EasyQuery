using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ezreal.EasyQuery.Interpreters
{
    public class OrderConditionArgumentsInterpreter
    {
        /// <summary>
        /// 校验约束
        /// </summary>
        /// <param name="orderConditionArguments"></param>
        /// <param name="orderConditionFilterAttribute"></param>
        /// <returns></returns>
        public virtual OrderConditionArguments CheckConstraint(OrderConditionArguments orderConditionArguments,
            List<OrderConditionFilterAttribute> orderConditionFilterAttribute = null)
        {
            if (orderConditionArguments == null) throw new ArgumentNullException(nameof(orderConditionArguments));

            orderConditionFilterAttribute = orderConditionFilterAttribute ?? new List<OrderConditionFilterAttribute>();
            List<OrderCondition> removeList = orderConditionArguments.Where(item =>
                orderConditionFilterAttribute != null && !orderConditionFilterAttribute.Exists(f =>
                    (f.ColumnName == null
                     || (f.ColumnName != null && f.ColumnName.Contains(item.ColumnName))
                    ) && (f.AllowOrderMode & item.OrderMode) == item.OrderMode)).ToList();
            removeList.ForEach(item => orderConditionArguments.Remove(item));
            return orderConditionArguments;
        }
    }
}
