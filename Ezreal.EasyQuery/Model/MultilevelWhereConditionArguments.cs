using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Ezreal.EasyQuery.Model
{
    public abstract class MultilevelWhereConditionArguments : IMultilevelArguments
    {
        /// <summary>
        /// 内联参数
        /// </summary>
        public virtual List<IMultilevelArguments> InternalMultilevelArguments { get; set; }


        protected List<WhereConditionFilterAttribute> _whereParameterAttributeList;
        public virtual void InvokeParameterFilter(IEnumerable<WhereConditionFilterAttribute> whereParameterAttributes)
        {
            if (_whereParameterAttributeList.IsNullOrNoItems())
            {
                _whereParameterAttributeList = whereParameterAttributes.ToList();
            }
        }
        public virtual void InitializeFromJsonObjectString(string jsonObjectString)
        {
            MultilevelWhereConditionArguments multilevelWhereConditionArguments = JsonConvert.DeserializeObject<MultilevelWhereConditionArguments>(jsonObjectString);
            Initialize(multilevelWhereConditionArguments);
        }

        public void Initialize(MultilevelWhereConditionArguments multilevelWhereConditionArguments)
        {
            this.InternalMultilevelArguments = new List<IMultilevelArguments>();
            foreach (var item in multilevelWhereConditionArguments.InternalMultilevelArguments)
            {
                if (item is WhereConditionArguments whereCondition)
                {
                    this.InternalMultilevelArguments.Add(whereCondition);
                    whereCondition.Initialize(whereCondition);
                }
                if (item is MultilevelWhereConditionArguments multilevelWhereCondition)
                {
                    this.InternalMultilevelArguments.Add(multilevelWhereCondition);
                    multilevelWhereConditionArguments.Initialize(multilevelWhereCondition);
                }
            }
        }
    }


}
