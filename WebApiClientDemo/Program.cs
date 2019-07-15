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
            var list = HttpApi.Create<IUserContract>().GetList(new WhereConditionArguments<User>()
            {
                LikeArguments = new List<WhereCondition>() {
                    new Ezreal.EasyQuery.Model.WhereCondition(){
                        ColumnName=nameof(User.Name),
                        ColumnValue="测",
                    },
                    //将被过滤
                    new Ezreal.EasyQuery.Model.WhereCondition(){
                        ColumnName=nameof(User.Age),
                        ColumnValue="1",
                    }
                }

            }).ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            Console.ReadKey();
        }
    }
}
