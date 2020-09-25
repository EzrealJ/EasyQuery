#if NETSTANDARD2_0
using System;
using Ezreal.EasyQuery.ModelBinders;
using Ezreal.EasyQuery.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Ezreal.EasyQuery.ModelBinderProviders
{
    public class WhereConditionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType.IsGenericType &&
                context.Metadata.ModelType.GetGenericTypeDefinition() == typeof(WhereConditionArguments<>))
            {
                return new BinderTypeModelBinder(typeof(WhereConditionModelBinder));
            }

            return null;
        }
    }
}

#endif