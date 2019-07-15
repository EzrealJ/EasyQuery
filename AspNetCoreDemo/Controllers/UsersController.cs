using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreDemo.Models;
using Ezreal.EasyQuery.Model;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.Controllers
{
    [Route("api/[controller]")]

    public class UsersController : ControllerBase
    {
        [HttpPost("GetList")]
        public async Task<IEnumerable<User>> GetList(
     [WhereConditionFilter(WhereConditionArguments.enumWherePattern.Like,nameof(Models.User.Name))]
     [FromForm]WhereConditionArguments<User> whereParameterArguments = null,
    [FromForm]OrderConditionArguments<User> orderConditionArguments = null)
        {
            Console.WriteLine(whereParameterArguments.GetWhereLambdaExpression<User>().ToString());
            return await Task.FromResult(new List<User>() {
                new User{ Name="AAA",Age=1},
                new User{ Name="BBB",Age=2},
            });
        }
    }
}
