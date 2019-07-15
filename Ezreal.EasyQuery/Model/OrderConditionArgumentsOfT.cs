using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class OrderConditionArguments<TSource> : OrderConditionArguments
    {
        protected List<PropertyInfo> _sourcePropertyInfos = typeof(TSource).GetProperties().ToList();
        public override void SetParameter(OrderCondition orderCondition)
        {
            if (!_sourcePropertyInfos.Exists(spi => spi.Name.Equals(orderCondition.ColumnName))) return;
            base.SetParameter(orderCondition);
        }

        protected override void Initialize(List<OrderCondition> orderConditionList)
        {
            orderConditionList = orderConditionList.Where(op => _sourcePropertyInfos.Exists(spi => spi.Name.Equals(op.ColumnName))).ToList();
            base.Initialize(orderConditionList);
        }


    }
}
