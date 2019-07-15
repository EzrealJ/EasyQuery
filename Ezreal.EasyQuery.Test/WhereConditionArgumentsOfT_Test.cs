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
            whereConditionArguments.EqualArguments = new List<WhereCondition>() {
                new WhereCondition(){ ColumnName=nameof(TestClassA.A),ColumnValue=1},
                new WhereCondition(){ ColumnName=nameof(TestClassA.B),ColumnValue=2}
            };

            whereConditionArguments.InArguments = new List<WhereCondition>() {
                new WhereCondition(){ ColumnName=nameof(TestClassA.A),ColumnValue=new int[]{ 1,2,3} },
                new WhereCondition(){ ColumnName=nameof(TestClassA.B),ColumnValue=new int[]{ 4,5,7}}
            };


            whereConditionArguments.LikeArguments = new List<WhereCondition>() {
                new WhereCondition(){ ColumnName=nameof(TestClassA.C),ColumnValue="1" },
                new WhereCondition(){ ColumnName=nameof(TestClassA.D),ColumnValue="2" }
            };

            var a = whereConditionArguments.GetWhereLambdaExpression();
        }
    }


}
