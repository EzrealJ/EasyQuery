using BenchmarkDotNet.Attributes;
using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Test;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Ezreal.EasyQuery.Models;
using Ezreal.EasyQuery.Interpreters;

namespace Ezreal.EasyQuery.Benchmark
{
    [RPlotExporter, RankColumn]
    public class WhereConditionArgumentsInterpret_Test
    {
        [Benchmark]
        public void GetWhereExpressionTest()
        {
            WhereConditionArguments<TestClassA> whereConditionArguments = new WhereConditionArguments<TestClassA>();
            whereConditionArguments.SpliceMode = Enums.EnumSpliceMode.AndAlso;
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { ColumnName = "A24235634", ColumnValue = "", MatchMode = Enums.EnumMatchMode.Equal });
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.A), ColumnValue = "1", MatchMode = Enums.EnumMatchMode.NotEqual });
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.A), ColumnValue = new int[] { 1, 2, 3, 4 }, MatchMode = Enums.EnumMatchMode.In });
            WhereConditionArguments<TestClassA> w2 = new WhereConditionArguments<TestClassA>();
            w2.SpliceMode = Enums.EnumSpliceMode.OrElse;
            w2.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.B), ColumnValue = "30", MatchMode = Enums.EnumMatchMode.NotEqual });
            w2.WhereConditions.Add(new WhereCondition() { ColumnName = "DDDDD", ColumnValue = "", MatchMode = Enums.EnumMatchMode.In });
            w2.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.A), ColumnValue = new int[] { 1, 2, 3, 4 }, MatchMode = Enums.EnumMatchMode.In });
            whereConditionArguments.InnerWhereConditionArguments.Add(w2);

            whereConditionArguments = JsonConvert.DeserializeObject<WhereConditionArguments<TestClassA>>(JsonConvert.SerializeObject(whereConditionArguments));


            List<WhereConditionFilterAttribute> whereConditionFilterAttributes = new List<WhereConditionFilterAttribute>();
            whereConditionFilterAttributes.Add(new WhereConditionFilterAttribute(Enums.EnumMatchMode.All, nameof(TestClassA.A), nameof(TestClassA.B)));


            var whereConditionArgumentsInterpret = new WhereConditionArgumentsInterpreter();
            var a = whereConditionArgumentsInterpret.CheckConstraint(whereConditionArguments, whereConditionFilterAttributes);


            WhereConditionArguments b = whereConditionArgumentsInterpret.Parse(whereConditionArguments);
            whereConditionArguments = b as WhereConditionArguments<TestClassA>;
            var c = whereConditionArguments.GetWhereLambdaExpression();
        }
    }
}
