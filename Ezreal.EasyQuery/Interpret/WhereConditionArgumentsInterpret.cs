using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Enums;
using Ezreal.EasyQuery.Exception;
using Ezreal.EasyQuery.Model;
using Ezreal.Extension.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ezreal.EasyQuery.Interpret
{
    public class WhereConditionArgumentsInterpret
    {



        /// <summary>
        /// 校验约束
        /// </summary>
        /// <param name="whereConditionArguments"></param>
        /// <param name="whereConditionFilterList"></param>
        /// <returns></returns>
        public virtual WhereConditionArguments CheckConstraint(WhereConditionArguments whereConditionArguments, List<WhereConditionFilterAttribute> whereConditionFilterList = null)
        {
            if (whereConditionArguments.IsNull())
            {
                throw new CheckConstraintException("解释器未找到可解释的对象");
            }
            whereConditionFilterList = whereConditionFilterList ?? new List<WhereConditionFilterAttribute>();
            List<WhereCondition> removeList = new List<WhereCondition>();
            foreach (WhereCondition item in whereConditionArguments.WhereConditions)
            {
                if (item is WhereCondition whereCondition)
                {
                    if (!whereConditionFilterList.IsNullOrNoItems() && !whereConditionFilterList.Exists(f
                         =>
                  (f.ColumnName.IsNullOrNoItems() ||
                  (!f.ColumnName.IsNullOrNoItems() && f.ColumnName.Contains(whereCondition.ColumnName))
                  ) && (f.AllowEnumMatchPattern & whereCondition.MatchMode) == whereCondition.MatchMode))
                    {
                        removeList.Add(item);
                    }
                }
            }
            foreach (WhereConditionArguments item in whereConditionArguments.InnerWhereConditionArguments)
            {
                if (item is WhereConditionArguments innerWhereConditionArguments)
                {
                    innerWhereConditionArguments = CheckConstraint(innerWhereConditionArguments, whereConditionFilterList);
                }
            }
            removeList.ForEach(item => whereConditionArguments.WhereConditions.Remove(item));
            return whereConditionArguments;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="whereConditionArguments"></param>
        /// <returns></returns>
        public virtual WhereConditionArguments Parse(WhereConditionArguments whereConditionArguments)
        {
            if (whereConditionArguments.IsNull())
            {
                throw new CheckConstraintException("解释器未找到可解释的对象");
            }
            List<WhereCondition> removeList = new List<WhereCondition>();
            foreach (WhereCondition item in whereConditionArguments.WhereConditions)
            {
                if (item is null)
                {
                    continue;
                }

                if (!(item is WhereCondition whereCondition))
                {
                    continue;
                }
                if ((whereCondition.MatchMode & (EnumMatchMode.Between | EnumMatchMode.NotBetween | EnumMatchMode.In | EnumMatchMode.NotIn)) == whereCondition.MatchMode)
                {

                    List<object> targetList;
                    Type type = whereCondition.ColumnValue?.GetType();
                    if (type is null)
                    {
                        continue;
                    }
                    if (type == typeof(string))
                    {
                        targetList = JsonConvert.DeserializeObject<List<object>>(((string)whereCondition.ColumnValue)?.Trim());
                    }
                    else if (whereCondition.ColumnValue is System.Collections.IEnumerable array)
                    {
                        targetList = new List<object>();
                        foreach (object columnValueItem in array)
                        {
                            targetList.Add(columnValueItem);
                        }
                    }
                    else
                    {
                        targetList = null;
                    }

                    if (targetList.IsNullOrNoItems() || ((whereCondition.MatchMode & (EnumMatchMode.Between | EnumMatchMode.NotBetween)) == whereCondition.MatchMode && targetList.Count != 2))
                    {
                        removeList.Add(item);
                    }
                    whereCondition.ColumnValue = targetList;
                }
                else if ((whereCondition.MatchMode & EnumMatchMode.Like) == EnumMatchMode.Like)
                {
                    //No conversion required
                }

            }
            foreach (WhereConditionArguments item in whereConditionArguments.InnerWhereConditionArguments)
            {
                if (item is WhereConditionArguments innerWhereConditionArguments)
                {
                    innerWhereConditionArguments = Parse(innerWhereConditionArguments);
                }
            }
            removeList.ForEach(item => whereConditionArguments.WhereConditions.Remove(item));
            return whereConditionArguments;


        }
    }
}
