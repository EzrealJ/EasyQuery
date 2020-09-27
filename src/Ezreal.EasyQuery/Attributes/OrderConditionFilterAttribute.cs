using Ezreal.EasyQuery.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class OrderConditionFilterAttribute : Attribute
    {
        public OrderConditionFilterAttribute(EnumOrderMode pattern, params string[] columnName)
        {
            AllowOrderMode = pattern;
            ColumnName = columnName;
        }
        /// <summary>
        /// 允许的排序模式
        /// </summary>
        public EnumOrderMode AllowOrderMode { get; }
        public string[] ColumnName { get; }
    }
}
