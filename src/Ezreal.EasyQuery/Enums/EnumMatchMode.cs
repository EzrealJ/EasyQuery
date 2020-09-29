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
        Equal = 0b1,
        NotEqual = 0b10,
        Like = 0b100,
        NotLike = 0b1000,
        LessThan = 0b10000,
        LessThanOrEqual = 0b100000,
        GreaterThan = 0b1000000,
        GreaterThanOrEqual = 0b10000000,
        Between = 0b100000000,
        NotBetween = 0b1000000000,
        In = 0b10000000000,
        NotIn = 0b100000000000,
        StartWith = 0b1000000000000,
        EndWith = 0b10000000000000,
        All = 0b11111111111111,
    }
}