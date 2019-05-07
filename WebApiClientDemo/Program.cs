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
            var list = HttpApi.Create<IUserContract>().GetList(new WhereParameterArguments<User>()
            {
                LikeArguments = new List<WhereParameter>() {
                    new Ezreal.EasyQuery.Model.WhereParameter(){
                        ColumnName=nameof(User.Name),
                        ColumnValue="测",
                        Pattern=Ezreal.EasyQuery.Model.WhereParameter.enumJoinPattern.And
                    },
                    //将被过滤
                    new Ezreal.EasyQuery.Model.WhereParameter(){
                        ColumnName=nameof(User.Age),
                        ColumnValue="1",
                        Pattern=Ezreal.EasyQuery.Model.WhereParameter.enumJoinPattern.And
                    }
                }

            }).ConfigureAwait(false).GetAwaiter().GetResult();

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(list));
            Console.ReadKey();
        }
    }
}
