using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Enums
{
    /// <summary>
    /// 排序方式枚举
    /// </summary>
    [Flags]
    public enum EnumOrderMode
    {
        /// <summary>
        /// 正序
        /// </summary>
        Asc = 0b1,
        /// <summary>
        /// 倒序
        /// </summary>
        Desc = 0b10,
        /// <summary>
        /// 排序方式均可被接受
        /// </summary>
        All = 0b11,
    }
}
