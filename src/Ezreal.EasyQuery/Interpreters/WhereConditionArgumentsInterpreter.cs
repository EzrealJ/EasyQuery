using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Enums;
using Ezreal.EasyQuery.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ezreal.EasyQuery.Interpreters
{
    public class WhereConditionArgumentsInterpreter
    {
        /// <summary>
        /// 校验约束
        /// </summary>
        /// <param name="whereConditionArguments"></param>
        /// <param name="whereConditionFilterList"></param>
        /// <returns></returns>
        public virtual WhereConditionArguments CheckConstraint(WhereConditionArguments whereConditionArguments,
            List<WhereConditionFilterAttribute> whereConditionFilterList = null)
        {
            if (whereConditionArguments == null) throw new ArgumentNullException(nameof(whereConditionArguments));

            whereConditionFilterList = whereConditionFilterList ?? new List<WhereConditionFilterAttribute>();
            List<WhereCondition> removeList = new List<WhereCondition>();
            foreach (WhereCondition item in whereConditionArguments.WhereConditions)
            {
                if (!(item is WhereCondition whereCondition))
                {
                    continue;
                }

                if (!whereConditionFilterList.Exists(f
                    =>
                    (f.ColumnName == null ||
                     (f.ColumnName != null && f.ColumnName.Contains(whereCondition.Key))
                    ) && (f.AllowEnumMatchPattern & whereCondition.MatchMode) == whereCondition.MatchMode))
                {
                    removeList.Add(item);
                }
            }

            foreach (WhereConditionArguments item in whereConditionArguments.InnerWhereConditionArguments)
            {
                if (item is WhereConditionArguments innerWhereConditionArguments)
                {
                    innerWhereConditionArguments =
                        CheckConstraint(innerWhereConditionArguments, whereConditionFilterList);
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
            if (whereConditionArguments == null) throw new ArgumentNullException(nameof(whereConditionArguments));


            List<WhereCondition> removeList = new List<WhereCondition>();
            foreach (WhereCondition item in whereConditionArguments.WhereConditions.Where(item => !(item is null)))
            {
                WhereCondition whereCondition = item;
                if ((whereCondition.MatchMode & (EnumMatchMode.Between | EnumMatchMode.NotBetween | EnumMatchMode.In |
                                                 EnumMatchMode.NotIn)) == whereCondition.MatchMode)
                {
                    List<object> targetList;
                    Type type = whereCondition.Value?.GetType();
                    if (type is null)
                    {
                        continue;
                    }

                    if (type == typeof(string))
                    {
                        targetList =
                            JsonConvert.DeserializeObject<List<object>>(((string)whereCondition.Value)?.Trim());
                    }
                    else if (whereCondition.Value is IEnumerable array)
                    {
                        targetList = array.Cast<object>().ToList();
                    }
                    else
                    {
                        targetList = null;
                    }

                    if (targetList == null ||
                        ((whereCondition.MatchMode & (EnumMatchMode.Between | EnumMatchMode.NotBetween)) ==
                            whereCondition.MatchMode && targetList.Count != 2))
                    {
                        removeList.Add(item);
                    }

                    whereCondition.Value = targetList;
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
