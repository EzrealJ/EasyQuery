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
            var arg = new WhereConditionArguments<User>();
            arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Name), ColumnValue = "AAA", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Equal });
            arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Name), ColumnValue = "BBB", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Like });
            arg.WhereConditions.Add(new WhereCondition() { ColumnName = nameof(User.Age), ColumnValue = "1", MatchMode = Ezreal.EasyQuery.Enums.EnumMatchMode.Equal });
            var order = new OrderConditionArguments<User>();
            order.Add(new OrderCondition() { ColumnName = nameof(User.Name), OrderMode = Ezreal.EasyQuery.Enums.EnumOrderMode.Asc });
            var list = HttpApi.Create<IUserContract>().GetList(arg, order).ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            Console.ReadKey();
        }
    }
}
