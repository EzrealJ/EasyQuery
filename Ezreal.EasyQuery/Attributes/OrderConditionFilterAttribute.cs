using Ezreal.EasyQuery.Enums;
using Ezreal.EasyQuery.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class OrderConditionFilterAttribute : System.Attribute
    {
        public OrderConditionFilterAttribute(EnumOrderMode pattern, params string[] columnName)
        {
            this.AllowOrderMode = pattern;
            this.ColumnName = columnName;
        }

        public EnumOrderMode AllowOrderMode { get; }
        public string[] ColumnName { get; }
    }
}
