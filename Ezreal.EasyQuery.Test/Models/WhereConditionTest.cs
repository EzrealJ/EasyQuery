using Ezreal.EasyQuery.Models;
using Ezreal.EasyQuery.Test.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xunit;

namespace Ezreal.EasyQuery.Test.Models
{
    [Trait("Category", "WhereConditionTest")]
    public class WhereConditionTest
    {
        [Fact]
        public void StaticFieldAssert()
        {
            var fields = typeof(WhereCondition).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var item in fields)
            {
                Assert.NotNull(item.GetValue(null));
            }

        }

        [Fact]
        public void TestGetExpression_Equal()
        {
            WhereCondition whereCondition = new WhereCondition();
            whereCondition.ColumnName = nameof(User.Name);
            whereCondition.ColumnValue = "用户一";
            whereCondition.MatchMode = Enums.EnumMatchMode.Equal;
            var parameter = Expression.Parameter(typeof(User), "u");
            var express = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(express);
            Assert.Equal(ExpressionType.Equal, express.NodeType);
        }
    }
}
