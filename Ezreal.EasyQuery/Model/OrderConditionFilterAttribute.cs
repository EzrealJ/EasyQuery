using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class OrderConditionFilterAttribute : System.Attribute
    {
        public OrderConditionFilterAttribute(OrderCondition.enumOrderPattern pattern, params string[] columnName)
        {
            this.Pattern = pattern;
            this.ColumnName = columnName;
        }

        public OrderCondition.enumOrderPattern Pattern { get; }
        public string[] ColumnName { get; }
    }
}
