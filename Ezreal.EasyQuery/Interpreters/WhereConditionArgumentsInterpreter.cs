using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Enums;
using Ezreal.EasyQuery.Models;
using Newtonsoft.Json;
using System;
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
            var removeList = new List<WhereCondition>();
            foreach (var item in whereConditionArguments.WhereConditions)
            {
                if (!(item is WhereCondition whereCondition)) continue;
                if (!whereConditionFilterList.Exists(f
                    =>
                    (f.ColumnName == null ||
                     (f.ColumnName != null && f.ColumnName.Contains(whereCondition.ColumnName))
                    ) && (f.AllowEnumMatchPattern & whereCondition.MatchMode) == whereCondition.MatchMode))
                {
                    removeList.Add(item);
                }
            }

            foreach (var item in whereConditionArguments.InnerWhereConditionArguments)
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


            var removeList = new List<WhereCondition>();
            foreach (var item in whereConditionArguments.WhereConditions.Where(item => !(item is null)))
            {
                var whereCondition = item;
                if ((whereCondition.MatchMode & (EnumMatchMode.Between | EnumMatchMode.NotBetween | EnumMatchMode.In |
                                                 EnumMatchMode.NotIn)) == whereCondition.MatchMode)
                {
                    List<object> targetList;
                    var type = whereCondition.ColumnValue?.GetType();
                    if (type is null)
                    {
                        continue;
                    }

                    if (type == typeof(string))
                    {
                        targetList =
                            JsonConvert.DeserializeObject<List<object>>(((string) whereCondition.ColumnValue)?.Trim());
                    }
                    else if (whereCondition.ColumnValue is System.Collections.IEnumerable array)
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

                    whereCondition.ColumnValue = targetList;
                }
                else if ((whereCondition.MatchMode & EnumMatchMode.Like) == EnumMatchMode.Like)
                {
                    //No conversion required
                }
            }

            foreach (var item in whereConditionArguments.InnerWhereConditionArguments)
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