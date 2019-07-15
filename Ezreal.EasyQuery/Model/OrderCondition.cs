using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ezreal.EasyQuery.Model
{
    public class OrderCondition
    {
        /// <summary>
        /// 排序列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 排序模式["asc"/"desc"]
        /// </summary>
        public enumOrderPattern Pattern { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        [Flags]
        public enum enumOrderPattern
        {
            Asc = 1,
            Desc = 2,
            All = 3,
        }

    }






}
