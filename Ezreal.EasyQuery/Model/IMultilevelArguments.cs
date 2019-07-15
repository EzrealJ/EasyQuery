using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    /// <summary>
    /// 表示多层次的参数
    /// </summary>
    public interface IMultilevelArguments:IEasyQueryModel
    {
        void InvokeParameterFilter(IEnumerable<WhereConditionFilterAttribute> whereParameterAttributes);

       

    }

    public interface IMultilevelArguments<TSource> : IMultilevelArguments
    {

    }

}
