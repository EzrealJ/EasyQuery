using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Models
{
    /// <summary>
    /// 将一个字符串通过包装起来以避免直接访问
    /// <para>
    /// 流行的ORM，比如EntityFramework，在翻译表达式时，这样包装字符串会使得ORM将其参数化,以避免字符串值带来的sql注入风险
    /// </para>
    /// </summary>
    public class ConstantStringWrapper
    {
        /// <summary>
        /// 获取常量值
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// 文本常量
        /// </summary>
        /// <param name="value">常量值</param>
        public ConstantStringWrapper(object value)
        {
            Value = value?.ToString();
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $@"""{Value}""";
        }
    }
}
