using AspNetCoreDemo.Models;
using Ezreal.EasyQuery.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using WebApiClient;

namespace WebApiClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(10 * 1000);
            while (true)
            {
                Console.ReadKey();
                string json = @"{""innerWhereConditionArguments"":[{""innerWhereConditionArguments"":[],""whereConditions"":[{""columnName"":""ID"",""columnValue"":""d756f49d-8efc-4de5-b040-703b3d9c9e4b"",""matchMode"":1}],""spliceMode"":1}],""whereConditions"":[],""spliceMode"":1}";
                var a = Newtonsoft.Json.JsonConvert.DeserializeObject<WhereConditionArguments<User>>(json);
                a.GetWhereLambdaExpression();




                var arg = new WhereConditionArguments<User>();
                arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Name), ColumnValue = "AAA", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Equal });
                arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Name), ColumnValue = "BBB", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Like });
                arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Age), ColumnValue = "1,2,3", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.In });
                var order = new OrderConditionArguments<User>();
                order.Add(new OrderCondition(nameof(User.Age), Ezreal.EasyQuery.Enums.EnumOrderMode.Asc));
                var list = HttpApi.Create<IUserContract>().GetList(a, order).ConfigureAwait(false).GetAwaiter().GetResult();

                Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list));

            }
        }



    }
}
