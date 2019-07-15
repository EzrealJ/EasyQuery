using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Enums
{
    /// <summary>
    /// 排序方式
    /// </summary>
    [Flags]
    public enum EnumOrderMode
    {
        Asc = 1,
        Desc = 2,
        All = 3,
    }
}
