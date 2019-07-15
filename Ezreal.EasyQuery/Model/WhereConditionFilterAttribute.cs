using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public class WhereConditionFilterAttribute : Attribute
    {

        public WhereConditionFilterAttribute(
    WhereConditionArguments.enumWherePattern wherePattern = WhereConditionArguments.enumWherePattern.Equal)
        {
            this.WherePattern = wherePattern;
        }
        public WhereConditionFilterAttribute(
            WhereConditionArguments.enumWherePattern wherePattern = WhereConditionArguments.enumWherePattern.Equal, params string[] columnName)
        {
            ColumnName = columnName;
            this.WherePattern = wherePattern;
        }

        /// <summary>
        /// 搜索列
        /// </summary>
        public virtual IEnumerable<string> ColumnName { get; set; }
        /// <summary>
        /// 匹配方式
        /// </summary>
        public virtual WhereConditionArguments.enumWherePattern WherePattern { get; private set; }


    }
}
