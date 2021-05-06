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
            AllowMatchPattern = wherePattern;
        }
        public WhereConditionFilterAttribute(
            EnumMatchMode wherePattern = EnumMatchMode.Equal, params string[] keys)
        {
            Keys = keys;
            AllowMatchPattern = wherePattern;
        }

        /// <summary>
        /// 搜索列
        /// </summary>
        public virtual IEnumerable<string> Keys { get; }
        /// <summary>
        /// 允许的匹配方式
        /// </summary>
        public virtual EnumMatchMode AllowMatchPattern { get; }


    }
}
