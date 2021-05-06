using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Enums;
using Ezreal.EasyQuery.Models;
using Newtonsoft.Json;

namespace Ezreal.EasyQuery.Interpreters
{
    public class WhereConditionArgumentsInterpreter
    {
        /// <summary>
        /// 应用过滤器
        /// </summary>
        /// <param name="whereConditionArguments">待处理参数</param>
        /// <param name="whereConditionFilterList">过滤器列表</param>
        /// <returns></returns>
        public virtual WhereConditionArguments ApplyFilter(WhereConditionArguments whereConditionArguments,
            List<WhereConditionFilterAttribute> whereConditionFilterList = null)
        {
            if (whereConditionArguments == null)
            {
                throw new ArgumentNullException(nameof(whereConditionArguments));
            }

            //没有过滤器约束则不进行任何处理
            if (whereConditionFilterList == null || whereConditionFilterList.Count == 0)
            {
                return whereConditionArguments;
            }

            List<WhereCondition> removeList = new List<WhereCondition>();
            foreach (WhereCondition item in whereConditionArguments.WhereConditions)
            {
                if (!(item is WhereCondition whereCondition))
                {
                    continue;
                }

                if (!whereConditionFilterList.Any(f
                    =>//若当前被审查元素不满足以下条件，则应被过滤
                    (f.Keys == null || f.Keys.Contains(whereCondition.Key))//未定义或者已包含
                    &&
                    (f.AllowMatchPattern & whereCondition.MatchMode) == whereCondition.MatchMode)//允许的匹配模式能够匹配
                    )
                {
                    removeList.Add(item);
                }
            }

            foreach (WhereConditionArguments item in whereConditionArguments.InnerWhereConditionArguments)
            {
                if (item is WhereConditionArguments innerWhereConditionArguments)
                {
                    innerWhereConditionArguments =
                        ApplyFilter(innerWhereConditionArguments, whereConditionFilterList);
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
                    IEnumerable<object> targetList;
                    Type type = whereCondition.Value?.GetType();
                    if (type is null)
                    {
                        continue;
                    }

                    targetList = type == typeof(string)
                        ? JsonConvert.DeserializeObject<List<object>>(((string)whereCondition.Value)?.Trim())
                        : whereCondition.Value is IEnumerable array ? array.Cast<object>() : null;

                    if (targetList == null ||
                        ((whereCondition.MatchMode & (EnumMatchMode.Between | EnumMatchMode.NotBetween)) ==
                            whereCondition.MatchMode && targetList.Count() != 2))
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
