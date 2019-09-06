using System;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Contexts;

namespace WebApiClientDemo
{
    public class JsonPathQueryFieldAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            FormatOptions options = context.HttpApiConfig.FormatOptions;
            string json = context.HttpApiConfig.JsonFormatter.Serialize(parameter.Value, options);
            string fieldName = parameter.Name;
            context.RequestMessage.AddUrlQuery(fieldName, json);
#if NET45
            await Task.FromResult<object>(null);
#else
            await Task.CompletedTask;
#endif
        }
    }
}
