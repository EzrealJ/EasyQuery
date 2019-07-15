using Ezreal.EasyQuery.Enums;
using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public class WhereConditionArguments : IMultilevelable
    {
        public List<WhereConditionArguments> InnerWhereConditionArguments { get; set; } = new List<WhereConditionArguments>();
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
            if (this.WhereConditions.IsNullOrNoItems() && this.InnerWhereConditionArguments.IsNullOrNoItems())
            {
                return null;
            }
            Expression where = null;

            foreach (var item in this.WhereConditions)
            {
                if (item is WhereCondition condition)
                {
                    where = SpliceExpression(where, condition.GetExpression<TSource>(parameter));
                }
            }

            foreach (var item in this.InnerWhereConditionArguments)
            {
                if (item is WhereConditionArguments conditionArguments)
                {
                    where = SpliceExpression(where, conditionArguments.GetExpression<TSource>(parameter));
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

            if (this.SpliceMode == EnumSpliceMode.AndAlso)
            {
                return Expression.AndAlso(left, right);
            }
            else
            {
                return Expression.OrElse(left, right);
            }
        }

    }
}
