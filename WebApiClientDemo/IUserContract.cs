﻿using AspNetCoreDemo.Models;
using Ezreal.EasyQuery.Model;
using System.Collections.Generic;
using WebApiClient;
using WebApiClient.Attributes;

namespace WebApiClientDemo
{
    [WebApiClient.Attributes.HttpHost("http://127.0.0.1:10000/")]
    [WebApiClient.Attributes.TraceFilterAttribute(OutputTarget = WebApiClient.Attributes.OutputTarget.Console)]
    public interface IUserContract : WebApiClient.IHttpApi
    {

        [HttpPost("api/Users/GetList")]
        ITask<IEnumerable<User>> GetList([JsonMulitpartText]WhereParameterArguments<User> whereParameterArguments = null, [JsonMulitpartText]OrderParameterArguments<User> orderParameterArguments = null, System.TimeSpan? timeout = null, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

    
    }
}