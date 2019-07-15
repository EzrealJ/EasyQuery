#if NETSTANDARD2_0
using Ezreal.EasyQuery.Attributes;
using Ezreal.EasyQuery.Interpret;
using Ezreal.EasyQuery.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezreal.EasyQuery.ModelBinder
{
    public class OrderConditionModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var modelName = bindingContext.BinderModelName;
            if (string.IsNullOrEmpty(modelName))
            {
                modelName = bindingContext.ModelMetadata.Name;
            }
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                await Task.CompletedTask;
                return;
            }
            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            string requestString = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(requestString))
            {
                await Task.CompletedTask;
                return;
            }

            if (!requestString.StartsWith("["))
            {
                requestString = $"[{requestString}]";
            }

            OrderConditionArguments orderConditionArguments = JsonConvert.DeserializeObject(requestString, bindingContext.ModelType) as OrderConditionArguments;
            try
            {
                IEnumerable<OrderConditionFilterAttribute> orderConditionFilterAttribute = ((DefaultModelMetadata)bindingContext.ModelMetadata).Attributes.ParameterAttributes
                    .Where(attr => attr.GetType() == typeof(OrderConditionFilterAttribute)).Select(attr => attr as OrderConditionFilterAttribute);
                var orderConditionArgumentsInterpret = new OrderConditionArgumentsInterpret();
                orderConditionArguments = orderConditionArgumentsInterpret.CheckConstraint(orderConditionArguments, orderConditionFilterAttribute.ToList());

            }
            catch (System.Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex.Message);
                await Task.CompletedTask;
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(orderConditionArguments);
            await Task.CompletedTask;
            return;
        }
    }
}

#endif