using AspNetCoreDemo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Ezreal.EasyQuery.Models;
using WebApiClient;

namespace WebApiClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] a = new string[] { "ss", "yyy" };
            string[] b = new string[] { "ssa", "y,yy", "111" };
            string value1 = JsonConvert.SerializeObject(a);
            string value2 = JsonConvert.SerializeObject(b);
            Console.WriteLine();
            //Thread.Sleep(10 * 1000);
            //while (true)
            //{
            //    Console.ReadKey();
            //    string json = @"{""innerWhereConditionArguments"":[{""innerWhereConditionArguments"":[],""whereConditions"":[{""columnName"":""ID"",""columnValue"":""d756f49d-8efc-4de5-b040-703b3d9c9e4b"",""matchMode"":1}],""spliceMode"":1}],""whereConditions"":[],""spliceMode"":1}";
            //    var a = Newtonsoft.Json.JsonConvert.DeserializeObject<WhereConditionArguments<User>>(json);
            //    a.GetWhereLambdaExpression();




            //    var arg = new WhereConditionArguments<User>();
            //    arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Name), ColumnValue = "AAA", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Equal });
            //    arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Name), ColumnValue = "BBB", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Like });
            //    arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Age), ColumnValue = "1,2,3", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.In });
            //    var order = new OrderConditionArguments<User>();
            //    order.Add(new OrderCondition(nameof(User.Age), Ezreal.EasyQuery.Enums.EnumOrderMode.Asc));
            //    var list = HttpApi.Create<IUserContract>().GetList(a, order).ConfigureAwait(false).GetAwaiter().GetResult();

            //    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list));

            //}





            Expression<Func<User, bool>> func = u => u.Age > 10 && u.Id == Guid.Empty && (u.Name.Contains("王") || u.Name.Contains("李"));
            var x = func.Body;
            GetWhereConditionArguments<User>(func.Body as BinaryExpression);
            Console.ReadKey();
        }


        public static WhereConditionArguments<TSource> GetWhereConditionArguments<TSource>(BinaryExpression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.NodeType != ExpressionType.AndAlso && expression.NodeType != ExpressionType.OrElse)
            {
                return null;
            }
            ExpressionType currentNodeType = expression.NodeType;
            Expression currentLeft = expression;
            WhereConditionArguments<TSource> currentLevel = new WhereConditionArguments<TSource>()
            {
                SpliceMode = expression.NodeType == ExpressionType.AndAlso ? Ezreal.EasyQuery.Enums.EnumSpliceMode.AndAlso : Ezreal.EasyQuery.Enums.EnumSpliceMode.OrElse
            };
            do
            {

                Expression currentRight = (currentLeft as BinaryExpression)?.Right ?? currentLeft;
                if (currentRight.NodeType == ExpressionType.AndAlso || currentRight.NodeType == ExpressionType.OrElse)
                {
                    WhereConditionArguments<TSource> innerItem = GetWhereConditionArguments<TSource>(currentRight as BinaryExpression);
                    if (innerItem != null)
                    {
                        currentLevel.InnerWhereConditionArguments.Add(innerItem);
                    }
                }
                else if (currentRight is MethodCallExpression methodCallExpression)
                {
                    Enum.TryParse<Ezreal.EasyQuery.Enums.EnumMatchMode>(methodCallExpression.Method.Name, out Ezreal.EasyQuery.Enums.EnumMatchMode matchMode);
                    WhereCondition whereCondition = new WhereCondition();
                    if (methodCallExpression.Object is MemberExpression member)
                    {
                        whereCondition.ColumnName = member.Member.Name;
                        whereCondition.ColumnValue = methodCallExpression.Arguments.FirstOrDefault().ToString();
                    }

                }
                else
                {

                    //解析

                    Console.WriteLine(currentRight.ToString());
                    Console.WriteLine(currentRight.NodeType);
                }
                currentLeft = (currentLeft as BinaryExpression)?.Left;
            }
            while (currentLeft != null);
            return currentLevel;
        }

    }
}
