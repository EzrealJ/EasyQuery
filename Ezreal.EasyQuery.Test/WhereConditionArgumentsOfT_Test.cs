using Ezreal.EasyQuery.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ezreal.EasyQuery.Test
{

    public class WhereConditionArgumentsOfT_Test
    {
        [Fact]
        public void GetWhereExpressionTest()
        {
            WhereConditionArguments<TestClassA> whereConditionArguments = new WhereConditionArguments<TestClassA>();
            whereConditionArguments.SpliceMode = Enums.EnumSpliceMode.AndAlso;
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.B), ColumnValue = "", MatchMode = Enums.EnumMatchMode.Equal });
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.A), ColumnValue = "1,2,3,4", MatchMode = Enums.EnumMatchMode.In });
            WhereConditionArguments<TestClassA> w2 = new WhereConditionArguments<TestClassA>();
            w2.SpliceMode = Enums.EnumSpliceMode.OrElse;
            w2.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.B), ColumnValue = "", MatchMode = Enums.EnumMatchMode.Equal });
            w2.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(TestClassA.B), ColumnValue = "1,2,3,4", MatchMode = Enums.EnumMatchMode.In });
            whereConditionArguments.InnerWhereConditionArguments.Add(w2);


            var a = whereConditionArguments.GetWhereLambdaExpression();
        }
    }


}
