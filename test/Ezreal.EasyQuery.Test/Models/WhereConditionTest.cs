using Ezreal.EasyQuery.Models;
using Ezreal.EasyQuery.Test.Data;
using Ezreal.EasyQuery.Test.Data.Enums;
using Ezreal.EasyQuery.Extensions;
using System;
using System.Linq.Expressions;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Ezreal.EasyQuery.Test.Models
{
    [Trait(nameof(WhereCondition), nameof(WhereConditionTest))]
    public class WhereConditionTest
    {
        private ITestOutputHelper TestOutputHelper { get; }

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
            Assert.True(expression is BinaryExpression);
            Assert.NotNull(((BinaryExpression)expression).Right);
            Expression right = ((BinaryExpression)expression).Right;
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

        private Expression AssertAndAlsoNullCheckExpressionOnLeftReturnRight(Expression expression)
        {
            Assert.NotNull(expression);
            Assert.Equal(ExpressionType.AndAlso, expression.NodeType);
            BinaryExpression exp = expression as BinaryExpression;
            Assert.NotNull(exp);
            BinaryExpression left = exp.Left as BinaryExpression;
            Assert.NotNull(left);
            Assert.Equal(ExpressionType.NotEqual, left.NodeType);
            Assert.Equal(ExpressionType.Constant, left.Right.NodeType);
            ConstantExpression lr = left.Right as ConstantExpression;
            Assert.NotNull(lr);
            Assert.Null(lr.Value);
            Expression right = exp.Right;
            Assert.NotNull(right);
            return right;
        }

        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.Equal))]
        [Theory]
        [InlineData(nameof(User.Name), "用户一", typeof(ConstantStringWrapper))]
        [InlineData(nameof(User.Id), "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData(nameof(User.DepartmentId), "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData(nameof(User.Age), "10", typeof(int))]
        [InlineData(nameof(User.Age), null, typeof(object))]
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


            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember() && value != null)
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }


            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.Equal, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }
        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.NotEqual))]
        [Theory]
        [InlineData(nameof(User.Name), "用户一", typeof(ConstantStringWrapper))]
        [InlineData(nameof(User.Id), "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData(nameof(User.DepartmentId), "082D4647-BAF5-481C-9D71-A68D9B3669FD", typeof(Guid))]
        [InlineData(nameof(User.Age), "10", typeof(int))]
        [InlineData(nameof(User.Age), null, typeof(object))]
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

            Assert.Equal(ExpressionType.Not, expression.NodeType);
            UnaryExpression exp = expression as UnaryExpression;
            Assert.NotNull(exp);
            Assert.NotNull(exp.Operand);


            expression = exp.Operand;
            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember() && value != null)
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }
            Assert.Equal(ExpressionType.Equal, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }

        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.Like))]
        [Theory]
        [InlineData(nameof(User.Name), "用户一")]
        [InlineData(nameof(User.Age), 12)]
        [InlineData(nameof(User.DepartmentId), "082D4647-BAF5-481C-9D71-A68D9B")]
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

            if (!typeof(ValueType).IsAssignableFrom(member.Type) || member.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
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
                Assert.NotNull(memberExpression);
                Type type = memberExpression.Member.DeclaringType;
                Assert.Equal(typeof(ConstantStringWrapper), type);
            }
        }
        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.NotLike))]
        [Theory]
        [InlineData(nameof(User.Name), "用户一")]
        [InlineData(nameof(User.Age), 12)]
        [InlineData(nameof(User.DepartmentId), "082D4647-BAF5-481C-9D71-A68D9B")]
        public void TestGetExpression_NotLike(string key, object value)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.NotLike
            };
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            MemberExpression member = Expression.PropertyOrField(parameter, key);
            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());


            Assert.Equal(ExpressionType.Not, expression.NodeType);
            UnaryExpression exp = expression as UnaryExpression;
            Assert.NotNull(exp);
            Assert.NotNull(exp.Operand);

            expression = exp.Operand;
            if (!typeof(ValueType).IsAssignableFrom(member.Type) || member.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
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
                Assert.NotNull(memberExpression);
                Type type = memberExpression.Member.DeclaringType;
                Assert.Equal(typeof(ConstantStringWrapper), type);
            }
        }

        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.LessThan))]
        [Theory]

        [InlineData(nameof(User.Age), 12, typeof(int))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", typeof(TimeSpan))]
        //[InlineData(nameof(User.State), "2", typeof(EnumUserState))]
        public void TestGetExpression_LessThan(string key, object value, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.LessThan
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.LessThan, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }

        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.LessThanOrEqual))]
        [Theory]

        [InlineData(nameof(User.Age), 12, typeof(int))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", typeof(TimeSpan))]
        //[InlineData(nameof(User.State), "2", typeof(EnumUserState))]
        public void TestGetExpression_LessThanOrEqual(string key, object value, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.LessThanOrEqual
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.LessThanOrEqual, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }

        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.GreaterThan))]
        [Theory]

        [InlineData(nameof(User.Age), 12, typeof(int))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", typeof(TimeSpan))]
        //[InlineData(nameof(User.State), "2", typeof(EnumUserState))]
        public void TestGetExpression_GreaterThan(string key, object value, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.GreaterThan
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.GreaterThan, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }

        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.GreaterThanOrEqual))]
        [Theory]

        [InlineData(nameof(User.Age), 12, typeof(int))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", typeof(TimeSpan))]
        //[InlineData(nameof(User.State), "2", typeof(EnumUserState))]
        public void TestGetExpression_GreaterThanOrEqual(string key, object value, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.GreaterThanOrEqual
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.GreaterThanOrEqual, expression.NodeType);
            AssertDeclarationTypeOfMemberExpressionOnRight(expression, targetType);
        }


        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.Between))]
        [Theory]

        [InlineData(nameof(User.Age), 12, 20, typeof(int))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", "09/28/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", "16:16:09.234", typeof(TimeSpan))]
        //[InlineData(nameof(User.State), "2", typeof(EnumUserState))]
        public void TestGetExpression_Between(string key, object value1, object value2, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = new[] { value1, value2 },
                MatchMode = Enums.EnumMatchMode.Between
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.Equal(ExpressionType.AndAlso, expression.NodeType);
            Assert.NotNull(expression as BinaryExpression);
            Expression leftExpression = (expression as BinaryExpression)?.Left;
            {
                TestOutputHelper.WriteLine($"leftExpression is {expression}");
                Assert.Equal(ExpressionType.GreaterThanOrEqual, leftExpression.NodeType);
                AssertDeclarationTypeOfMemberExpressionOnRight(leftExpression, targetType);
            }
            Expression rihtExpression = (expression as BinaryExpression)?.Right;
            {
                TestOutputHelper.WriteLine($"rihtExpression is {expression}");
                Assert.Equal(ExpressionType.LessThanOrEqual, rihtExpression.NodeType);
                AssertDeclarationTypeOfMemberExpressionOnRight(rihtExpression, targetType);
            }


        }


        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.NotBetween))]
        [Theory]

        [InlineData(nameof(User.Age), 12, 20, typeof(int))]
        [InlineData(nameof(User.CreateTime), "09/27/2020 02:22:00", "09/28/2020 02:22:00", typeof(DateTime))]
        [InlineData(nameof(User.TestProp1), "02:20:09.234", "16:16:09.234", typeof(TimeSpan))]
        //[InlineData(nameof(User.State), "2", typeof(EnumUserState))]
        public void TestGetExpression_NotBetween(string key, object value1, object value2, Type targetType)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = new[] { value1, value2 },
                MatchMode = Enums.EnumMatchMode.NotBetween
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());

            Assert.Equal(ExpressionType.Not, expression.NodeType);
            UnaryExpression exp = expression as UnaryExpression;
            Assert.NotNull(exp);
            Assert.NotNull(exp.Operand);
            expression = exp.Operand;

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.Equal(ExpressionType.AndAlso, expression.NodeType);
            Assert.NotNull(expression as BinaryExpression);
            Expression leftExpression = (expression as BinaryExpression)?.Left;
            {
                TestOutputHelper.WriteLine($"leftExpression is {expression}");
                Assert.Equal(ExpressionType.GreaterThanOrEqual, leftExpression.NodeType);
                AssertDeclarationTypeOfMemberExpressionOnRight(leftExpression, targetType);
            }
            Expression rihtExpression = (expression as BinaryExpression)?.Right;
            {
                TestOutputHelper.WriteLine($"rihtExpression is {expression}");
                Assert.Equal(ExpressionType.LessThanOrEqual, rihtExpression.NodeType);
                AssertDeclarationTypeOfMemberExpressionOnRight(rihtExpression, targetType);
            }


        }



        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.In))]
        [Theory]
        [InlineData(nameof(User.Age), typeof(int), 12, 20)]
        [InlineData(nameof(User.CreateTime), typeof(DateTime), "09/27/2020 02:22:00", "09/28/2020 02:22:00")]
        [InlineData(nameof(User.TestProp1), typeof(TimeSpan), "02:20:09.234", "16:16:09.234")]
        [InlineData(nameof(User.State), typeof(EnumUserState), 1, 2, 0)]
        [InlineData(nameof(User.Name), typeof(ConstantStringWrapper), "jack", "tom", "eric", "ezreal")]
        public void TestGetExpression_In(string key, Type targetType, params object[] values)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = values,
                MatchMode = Enums.EnumMatchMode.In
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }

            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());
            Assert.NotNull(expression as BinaryExpression);
            while (expression is BinaryExpression currentExpress)
            {
                if (currentExpress.NodeType == ExpressionType.Equal)
                {
                    TestOutputHelper.WriteLine("Last Expression is " + currentExpress);
                    AssertDeclarationTypeOfMemberExpressionOnRight(currentExpress, targetType);
                    break;
                }

                Assert.Equal(ExpressionType.OrElse, currentExpress.NodeType);
                expression = currentExpress.Left;
                Expression right = currentExpress.Right;
                TestOutputHelper.WriteLine("Current Right Expression is " + right);
                Assert.Equal(ExpressionType.Equal, right.NodeType);
                AssertDeclarationTypeOfMemberExpressionOnRight(right, targetType);
            }



        }


        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.NotIn))]
        [Theory]
        [InlineData(nameof(User.Age), typeof(int), 12, 20)]
        [InlineData(nameof(User.CreateTime), typeof(DateTime), "09/27/2020 02:22:00", "09/28/2020 02:22:00")]
        [InlineData(nameof(User.TestProp1), typeof(TimeSpan), "02:20:09.234", "16:16:09.234")]
        [InlineData(nameof(User.State), typeof(EnumUserState), 1, 2, 0)]
        [InlineData(nameof(User.Name), typeof(ConstantStringWrapper), "jack", "tom", "eric", "ezreal")]
        public void TestGetExpression_NotIn(string key, Type targetType, params object[] values)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = values,
                MatchMode = Enums.EnumMatchMode.NotIn
            };

            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");
            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());

            Assert.Equal(ExpressionType.Not, expression.NodeType);
            UnaryExpression exp = expression as UnaryExpression;
            Assert.NotNull(exp);
            Assert.NotNull(exp.Operand);
            expression = exp.Operand;

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }


            Assert.NotNull(expression as BinaryExpression);
            while (expression is BinaryExpression currentExpress)
            {
                if (currentExpress.NodeType == ExpressionType.Equal)
                {
                    TestOutputHelper.WriteLine("Last Expression is " + currentExpress);
                    AssertDeclarationTypeOfMemberExpressionOnRight(currentExpress, targetType);
                    break;
                }

                Assert.Equal(ExpressionType.OrElse, currentExpress.NodeType);
                expression = currentExpress.Left;
                Expression right = currentExpress.Right;
                TestOutputHelper.WriteLine("Current Right Expression is " + right);
                Assert.Equal(ExpressionType.Equal, right.NodeType);
                AssertDeclarationTypeOfMemberExpressionOnRight(right, targetType);
            }


        }











        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.StartWith))]
        [Theory]
        [InlineData(nameof(User.Name), "用户一")]
        [InlineData(nameof(User.Age), 12)]
        [InlineData(nameof(User.DepartmentId), "082D4647-BAF5-481C-9D71-A68D9B")]
        public void TestGetExpression_StartWith(string key, object value)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.StartWith
            };
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");

            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (!typeof(ValueType).IsAssignableFrom(memberAccessor.Type) || memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }


            Assert.NotNull(expression as MethodCallExpression);
            MethodCallExpression methodCallExpression = expression as MethodCallExpression;
            Assert.Equal(Methods.StringStartsWithMethod, methodCallExpression.Method);
            {

                Expression objExpression = methodCallExpression.Object;
                if (memberAccessor.Type != typeof(string))
                {
                    Assert.NotNull(objExpression as MethodCallExpression);
                    MethodCallExpression toStringMethodCallExpression = objExpression as MethodCallExpression;
                    Assert.Equal(Methods.ObjectToStringMethod, toStringMethodCallExpression.Method);
                    objExpression = toStringMethodCallExpression.Object;
                }

                Assert.NotNull(objExpression as MemberExpression);

            }
            {
                Expression objExpression = methodCallExpression.Arguments[0];
                Assert.NotNull(objExpression as MemberExpression);
                MemberExpression memberExpression = objExpression as MemberExpression;
                Assert.NotNull(memberExpression);
                Type type = memberExpression.Member.DeclaringType;
                Assert.Equal(typeof(ConstantStringWrapper), type);
            }


        }


        [Trait(nameof(Enums.EnumMatchMode), nameof(Enums.EnumMatchMode.EndWith))]
        [Theory]
        [InlineData(nameof(User.Name), "用户一")]
        //[InlineData(nameof(User.Age), 12)]
        //[InlineData(nameof(User.DepartmentId), "082D4647-BAF5-481C-9D71-A68D9B")]
        //[InlineData(nameof(User.Id), "082D4647-BAF5-481C-9D71-A68D9B")]
        public void TestGetExpression_EndWith(string key, object value)
        {
            WhereCondition whereCondition = new WhereCondition
            {
                Key = key,
                Value = value,
                MatchMode = Enums.EnumMatchMode.EndWith
            };
            ParameterExpression parameter = Expression.Parameter(typeof(User), "u");

            Expression expression = whereCondition.GetExpression<User>(parameter);
            Assert.NotNull(expression);
            TestOutputHelper.WriteLine(expression.ToString());

            MemberExpression memberAccessor = Expression.PropertyOrField(parameter, key);
            if (!typeof(ValueType).IsAssignableFrom(memberAccessor.Type) || memberAccessor.IsNullableStructMember())
            {
                expression = AssertAndAlsoNullCheckExpressionOnLeftReturnRight(expression);
            }


            Assert.NotNull(expression as MethodCallExpression);
            MethodCallExpression methodCallExpression = expression as MethodCallExpression;
            Assert.Equal(Methods.StringEndsWithMethod, methodCallExpression.Method);
            {

                Expression objExpression = methodCallExpression.Object;
                if (memberAccessor.Type != typeof(string))
                {
                    Assert.NotNull(objExpression as MethodCallExpression);
                    MethodCallExpression toStringMethodCallExpression = objExpression as MethodCallExpression;
                    Assert.Equal(Methods.ObjectToStringMethod, toStringMethodCallExpression.Method);
                    objExpression = toStringMethodCallExpression.Object;
                }

                Assert.NotNull(objExpression as MemberExpression);

            }
            {
                Expression objExpression = methodCallExpression.Arguments[0];
                Assert.NotNull(objExpression as MemberExpression);
                MemberExpression memberExpression = objExpression as MemberExpression;
                Assert.NotNull(memberExpression);
                Type type = memberExpression.Member.DeclaringType;
                Assert.Equal(typeof(ConstantStringWrapper), type);
            }


        }



    }
}
