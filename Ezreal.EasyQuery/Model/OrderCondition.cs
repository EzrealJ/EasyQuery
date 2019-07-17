using Ezreal.EasyQuery.Enums;
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


        public OrderCondition(string columnName, EnumOrderMode orderMode)
        {
            ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            OrderMode = orderMode;
        }

        /// <summary>
        /// 排序列
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 排序模式["asc"/"desc"]
        /// </summary>
        public EnumOrderMode OrderMode { get; set; }



    }






}
