using Ezreal.EasyQuery.Model;
using Ezreal.EasyQuery.ModelBinder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace Ezreal.EasyQuery.ModelBinderProvider
{
    public class WhereParameterModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType.IsGenericType &&
                context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(WhereParameterArguments<>))
            {
                return new BinderTypeModelBinder(typeof(WhereParameterModelBinder));
            }

            return null;
        }
    }
}
