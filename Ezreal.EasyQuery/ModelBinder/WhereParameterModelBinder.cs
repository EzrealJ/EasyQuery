using Ezreal.EasyQuery.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ezreal.EasyQuery.ModelBinder
{
    public class WhereParameterModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
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
                return Task.CompletedTask;
            }
            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            string requestString = valueProviderResult.FirstValue;
            if (string.IsNullOrEmpty(requestString))
            {
                return Task.CompletedTask;
            }
            WhereParameterArguments whereParameterArguments = Activator.CreateInstance(bindingContext.ModelType) as WhereParameterArguments;
            try
            {
                IEnumerable<WhereParameterAttribute> whereParameterAttributes = ((DefaultModelMetadata)bindingContext.ModelMetadata).Attributes.ParameterAttributes
                    .Where(attr => attr.GetType() == typeof(WhereParameterAttribute)).Select(attr => attr as WhereParameterAttribute);
                whereParameterArguments.InvokeParameterFilter(whereParameterAttributes);
                whereParameterArguments.InitializeFromJsonObjectString(requestString);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex.Message);
                return Task.CompletedTask;
            }

            bindingContext.Result = ModelBindingResult.Success(whereParameterArguments);
            return Task.CompletedTask;
        }
    }
}
