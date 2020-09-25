using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Ezreal.EasyQuery.Test.Data.Enums
{
    public enum EnumUserState
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Enable = 1,
        /// <summary>
        /// 未开户
        /// </summary>
        [Description("未开户")]
        UnRegister = 0,
        /// <summary>
        /// 挂失
        /// </summary>
        [Description("挂失")]
        Loss = 2,
        /// <summary>
        /// 注销
        /// </summary>
        [Description("注销")]
        Logout = 3,

    }
}
