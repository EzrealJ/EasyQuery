using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Ezreal.EasyQuery.Model
{


    public class WhereCondition
    {
        /// <summary>
        /// 搜索列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 搜索值
        /// </summary>
        public object ColumnValue { get; set; }

    }






}
