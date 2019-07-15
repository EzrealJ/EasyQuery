using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    /// <summary>
    /// 表示可作为多层次的参数
    /// </summary>
    public interface IMultilevelable : IEasyQueryModel
    {



    }

    public interface IMultilevelArguments<TSource> : IMultilevelable
    {
        Expression<Func<TSource, bool>> GetWhereLambdaExpression();
        Expression<Func<TDBOSource, bool>> GetWhereLambdaExpression<TDBOSource>();

    }

}
