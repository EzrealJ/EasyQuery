using System.Collections.Generic;
using System.Linq.Expressions;
using Ezreal.EasyQuery.Enums;

namespace Ezreal.EasyQuery.Models
{
    public class WhereConditionArguments
    {
        /// <summary>
        /// 内部组合条件的集合
        /// </summary>
        public List<WhereConditionArguments> InnerWhereConditionArguments { get; set; } =
            new List<WhereConditionArguments>();

        /// <summary>
        /// 平铺条件的集合
        /// </summary>
        public List<WhereCondition> WhereConditions { get; set; } = new List<WhereCondition>();

        /// <summary>
        /// 子项拼接模式
        /// <para>
        /// 可选And,Or
        /// </para>
        /// </summary>
        public EnumSpliceMode SpliceMode { get; set; }


        public Expression GetExpression<TSource>(ParameterExpression parameter)
        {
            if (this.WhereConditions == null && this.InnerWhereConditionArguments == null)
            {
                return null;
            }

            Expression where = null;

            if (this.WhereConditions != null)
            {
                foreach (var item in this.WhereConditions)
                {
                    if (item is WhereCondition condition)
                    {
                        where = SpliceExpression(where, condition.GetExpression<TSource>(parameter));
                    }
                }
            }
 
            foreach (var item in this.InnerWhereConditionArguments)
            {
                if (item is WhereConditionArguments conditionArguments)
                {
                    var right = conditionArguments.GetExpression<TSource>(parameter);
                    if (right != null)
                    {
                        where = SpliceExpression(where, right);
                    }
                }
            }

            return where;
        }

        private Expression SpliceExpression(Expression left, Expression right)
        {
            if (left == null)
            {
                return right;
            }

            return this.SpliceMode == EnumSpliceMode.AndAlso ? Expression.AndAlso(left, right) : Expression.OrElse(left, right);
        }
    }
}