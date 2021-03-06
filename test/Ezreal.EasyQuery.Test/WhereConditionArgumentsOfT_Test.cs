﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Ezreal.EasyQuery.Models;
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
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { Key = nameof(TestClassA.A), Value = "1", MatchMode = Enums.EnumMatchMode.Equal });
            whereConditionArguments.WhereConditions.Add(new WhereCondition() { Key = nameof(TestClassA.A), Value = "1,2,3,4", MatchMode = Enums.EnumMatchMode.In });
            WhereConditionArguments<TestClassA> w2 = new WhereConditionArguments<TestClassA>();
            w2.SpliceMode = Enums.EnumSpliceMode.OrElse;
            w2.WhereConditions.Add(new WhereCondition() { Key = nameof(TestClassA.A), Value = "1", MatchMode = Enums.EnumMatchMode.Equal });
            w2.WhereConditions.Add(new WhereCondition() { Key = nameof(TestClassA.B), Value = "1,2,3,4", MatchMode = Enums.EnumMatchMode.In });
            whereConditionArguments.InnerWhereConditionArguments.Add(w2);


            Expression<Func<TestClassA, bool>> a = whereConditionArguments.GetWhereLambdaExpression();
        }
    }


}
