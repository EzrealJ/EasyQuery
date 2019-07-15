#if NETSTANDARD2_0
using Ezreal.EasyQuery.ModelBinderProvider;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ezreal.EasyQuery
{
    public static class ModelBinderProvidersExtension
    {
        public static void AddEasyQuery(this IList<IModelBinderProvider> modelBinderProviders)
        {
            modelBinderProviders.Insert(0, new WhereConditionModelBinderProvider());
            modelBinderProviders.Insert(0, new OrderConditionModelBinderProvider());
        }
    }
}

#endif