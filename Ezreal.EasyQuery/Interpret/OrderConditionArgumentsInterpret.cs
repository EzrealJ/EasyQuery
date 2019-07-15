using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Exception;
using Ezreal.EasyQuery.Model;
using Ezreal.Extension.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ezreal.EasyQuery.Interpret
{
    public class OrderConditionArgumentsInterpret
    {
        /// <summary>
        /// 校验约束
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="wherePattern"></param>
        /// <returns></returns>
        public virtual OrderConditionArguments CheckConstraint(OrderConditionArguments orderConditionArguments, List<OrderConditionFilterAttribute> orderConditionFilterAttribute = null)
        {
            if (orderConditionArguments.IsNull())
            {
                throw new CheckConstraintException("解释器未找到可解释的对象");
            }
            orderConditionFilterAttribute = orderConditionFilterAttribute ?? new List<OrderConditionFilterAttribute>();
            List<OrderCondition> removeList = new List<OrderCondition>();
            foreach (OrderCondition item in orderConditionArguments)
            {

                if (!orderConditionFilterAttribute.Exists(f
                    => f.ColumnName.Contains(item.ColumnName)
                    && (f.AllowOrderMode & item.OrderMode) == item.OrderMode))
                {
                    removeList.Add(item);
                }

            }
            removeList.ForEach(item => orderConditionArguments.Remove(item));
            return orderConditionArguments;
        }

    }
}
