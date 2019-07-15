#if NETSTANDARD2_0
using Ezreal.EasyQuery.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ezreal.EasyQuery.ModelBinder
{
    public class MultilevelWhereParameterModelBinder : IModelBinder
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
            MultilevelWhereConditionArguments multilevelWhereConditionArguments = Activator.CreateInstance(bindingContext.ModelType) as MultilevelWhereConditionArguments;
            try
            {
                IEnumerable<WhereConditionFilterAttribute> whereParameterAttributes = ((DefaultModelMetadata)bindingContext.ModelMetadata).Attributes.ParameterAttributes
                    .Where(attr => attr.GetType() == typeof(WhereConditionFilterAttribute)).Select(attr => attr as WhereConditionFilterAttribute);
                multilevelWhereConditionArguments.InvokeParameterFilter(whereParameterAttributes);
                multilevelWhereConditionArguments.InitializeFromJsonObjectString(requestString);
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, ex.Message);
                await Task.CompletedTask;
                return;
            }

            bindingContext.Result = ModelBindingResult.Success(multilevelWhereConditionArguments);
            await Task.CompletedTask;
            return;
        }
    }
}
#endif