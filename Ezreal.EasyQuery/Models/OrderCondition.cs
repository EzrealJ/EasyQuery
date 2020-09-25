using System;
using Ezreal.EasyQuery.Enums;

namespace Ezreal.EasyQuery.Models
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
        public string ColumnName { get; }
        /// <summary>
        /// 排序模式["asc"/"desc"]
        /// </summary>
        public EnumOrderMode OrderMode { get; }



    }






}
