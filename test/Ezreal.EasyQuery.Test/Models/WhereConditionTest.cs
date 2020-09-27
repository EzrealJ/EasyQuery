using Ezreal.EasyQuery.Models;
using Ezreal.EasyQuery.Test.Data;
using Ezreal.EasyQuery.Test.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Ezreal.EasyQuery.Test.Models
{
    [Trait("Category", "WhereConditionTest")]
    public class WhereConditionTest
    {
        public ITestOutputHelper TestOutputHelper { get; }

        public WhereConditionTest(ITestOutputHelper testOutputHelper)
        {
            TestOutputHelper = testOutputHelper;
        }

        [Fact]
        public void StaticFieldAssert()
        {
            FieldInfo[] fields = typeof(WhereCondition).GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (FieldInfo item in fields)
            {
                Assert.NotNull(item.GetValue(null));
            }

        }

        private void AssertDeclarationTypeOfMemberExpressionOnRight(Expression expression, Type targetType)
        {
            Assert.True(typeof(BinaryExpression).IsAssignableFrom(expression.GetType()));
            Assert.NotNull((expression as BinaryExpression).Right);
            Expression right = (expression as BinaryExpression).Right;
            if (targetType == typeof(ConstantStringWrapper))
            {
                Assert.NotNull(right as MemberExpression);
                MemberExpression memberExpression = right as MemberExpression;
                Type type = memberExpression.Member.DeclaringType;
                Assert.Equal(targetType, type);
            }
            else
            {
                Assert.NotNull(right as ConstantExpression);
                ConstantExpression constantExpression = right as ConstantExpression;
                Type type = constantExpression.Type;
                Assert.Equal(targetType, type);
            }

        }

        [Theory]
        [InlineData("Name", "用户一", typeof(ConstantStringWrapper))]
        [InlineData("Id", "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData("DepartmentId", "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData("Age", "10", typeof(int))]
        [InlineData("Age", null, typeof(object))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", typeof(TimeSpan))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.State), "1", typeof(EnumUserState))]
        public void TestGetExpression_Equal(string key, object value, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.Equal
            };
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.Equal, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }

        [Theory]
        [InlineData("Name", "用户一", typeof(ConstantStringWrapper))]
        [InlineData("Id", "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData("DepartmentId", "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData("Age", "10", typeof(int))]
        [InlineData("Age", null, typeof(object))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", typeof(TimeSpan))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.State), "1", typeof(EnumUserState))]
        public void TestGetExpression_NotEqual(string key, object value, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.NotEqual
            };
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.NotEqual, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }


        [Theory]
        
        [InlineData("Name", "用户一")]
        [InlineData("Age", 12)]
        [InlineData("DepartmentId", "sfagfwe")]
        public void TestGetExpression_Like(string key, object value)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.Like
            };
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            MemberExpression member = Expression.PropertyOrField(parameter, key);
            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            if (typeof(ValueType).IsAssignableFrom(member.Type))
            {
                Assert.Equal(ExpressionType.Call, expression.NodeType);
            }
            else
            {
                Assert.Equal(ExpressionType.AndAlso, expression.NodeType);
                Expression leftExpression = (expression as BinaryExpression).Left;
                Assert.Equal(ExpressionType.NotEqual, leftExpression.NodeType);
                Assert.Null(((ConstantExpression)((BinaryExpression)leftExpression).Right).Value);
                expression = (expression as BinaryExpression).Right;
            }

            Assert.NotNull(expression as MethodCallExpression);
            MethodCallExpression methodCallExpression = expression as MethodCallExpression;
            Assert.Equal(Methods.StringContainsMethod, methodCallExpression.Method);
            {
                Expression objExpression = methodCallExpression.Object;
                Assert.NotNull(objExpression as MethodCallExpression);
                MethodCallExpression toLowerMethodCallExpression = objExpression as MethodCallExpression;
                Assert.Equal(Methods.StringToLowerMethod, toLowerMethodCallExpression.Method);
                {
                    objExpression = toLowerMethodCallExpression.Object;
                    if (member.Type != typeof(string))
                    {

                        Assert.NotNull(objExpression as MethodCallExpression);
                        MethodCallExpression toStringMethodCallExpression = objExpression as MethodCallExpression;
                        Assert.Equal(Methods.ObjectToStringMethod, toStringMethodCallExpression.Method);
                        objExpression = toStringMethodCallExpression.Object;
                    }

                    Assert.NotNull(objExpression as MemberExpression);
                }

            }
            {
                Expression objExpression = methodCallExpression.Arguments[0];
                Assert.NotNull(objExpression as MethodCallExpression);
                MethodCallExpression toLowerMethodCallExpression = objExpression as MethodCallExpression;
                Assert.Equal(Methods.StringToLowerMethod, toLowerMethodCallExpression.Method);
                Assert.NotNull(toLowerMethodCallExpression.Object as MemberExpression);
                MemberExpression memberExpression = toLowerMethodCallExpression.Object as MemberExpression;
                Type type = memberExpression.Member.DeclaringType;
                Assert.Equal(typeof(ConstantStringWrapper), type);
            }




        }
    }
}
