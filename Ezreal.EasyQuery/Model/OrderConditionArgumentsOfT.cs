using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class OrderConditionArguments<TSource> : OrderConditionArguments
    {
        public virtual IQueryable<TSource> GetOrderedQueryable(IQueryable<TSource> queryable) => base.GetOrderedQueryable<TSource>(queryable);
    }
}
