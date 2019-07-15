using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Enums
{
    /// <summary>
    /// 条件匹配模式的枚举
    /// </summary>
    [Flags]
    public enum EnumMatchMode
    {
        Equal = 1,
        NotEqual = 2,
        Like = 4,
        NotLike = 8,
        Less = 16,
        LessOrEqual = 32,
        More = 64,
        MoreOrEqual = 128,
        Between = 256,
        NotBetween = 512,
        In = 1024,
        NotIn = 2048,
        StartWith = 4096,
        EndWith = 8192,
        All = 16383,
    }
}
