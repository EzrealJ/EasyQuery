using Ezreal.EasyQuery.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class WhereConditionFilterAttribute : Attribute
    {

        public WhereConditionFilterAttribute(
    EnumMatchMode wherePattern = EnumMatchMode.Equal)
        {
            this.AllowEnumMatchPattern = wherePattern;
        }
        public WhereConditionFilterAttribute(
            EnumMatchMode wherePattern = EnumMatchMode.Equal, params string[] columnName)
        {
            ColumnName = columnName;
            this.AllowEnumMatchPattern = wherePattern;
        }

        /// <summary>
        /// 搜索列
        /// </summary>
        public virtual IEnumerable<string> ColumnName { get; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public virtual EnumMatchMode AllowEnumMatchPattern { get; }


    }
}
