using System.Linq;

namespace Ezreal.EasyQuery.Models
{
    public class OrderConditionArguments<TSource> : OrderConditionArguments
    {
        public virtual IQueryable<TSource> GetOrderedQueryable(IQueryable<TSource> queryable) => base.GetOrderedQueryable<TSource>(queryable);
    }
}
