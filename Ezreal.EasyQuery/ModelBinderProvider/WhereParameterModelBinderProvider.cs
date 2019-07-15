#if NETSTANDARD2_0
using Ezreal.EasyQuery.Model;
using Ezreal.EasyQuery.ModelBinder;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;

namespace Ezreal.EasyQuery.ModelBinderProvider
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