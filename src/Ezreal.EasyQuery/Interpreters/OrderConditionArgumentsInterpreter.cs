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
        /// <param name="orderConditionFilterAttributeList"></param>
        /// <returns></returns>
        public virtual OrderConditionArguments ApplyFilter(OrderConditionArguments orderConditionArguments,
            List<OrderConditionFilterAttribute> orderConditionFilterAttributeList = null)
        {
            if (orderConditionArguments == null)
            {
                throw new ArgumentNullException(nameof(orderConditionArguments));
            }

            //没有过滤器约束则不进行任何处理
            if (orderConditionFilterAttributeList == null || orderConditionFilterAttributeList.Count == 0)
            {
                return orderConditionArguments;
            }

            orderConditionFilterAttributeList = orderConditionFilterAttributeList ?? new List<OrderConditionFilterAttribute>();
            List<OrderCondition> removeList = new List<OrderCondition>();
            foreach (OrderCondition item in orderConditionArguments)
            {


                if (!orderConditionFilterAttributeList.Any(f
                    =>//若当前被审查元素不满足以下条件，则应被过滤
                    (f.ColumnName == null || f.ColumnName.Contains(item.ColumnName))//未定义或者已包含
                    &&
                    (f.AllowOrderMode & item.OrderMode) == item.OrderMode)//允许的匹配模式能够匹配
                    )
                {
                    removeList.Add(item);
                }
            }
            removeList.ForEach(item => orderConditionArguments.Remove(item));
            return orderConditionArguments;
        }
    }
}
