using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ezreal.EasyQuery.Test.Data
{
    internal class Methods
    {
        public static readonly MethodInfo ObjectToStringMethod =
    typeof(object).GetMethod(nameof(ToString), new Type[] { });

        public static readonly MethodInfo StringContainsMethod =
            typeof(string).GetMethod(nameof(string.Contains), new Type[] { typeof(string) });

        public static readonly MethodInfo StringStartsWithMethod =
            typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(string) });

        public static readonly MethodInfo StringEndsWithMethod =
            typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(string) });

        public static readonly MethodInfo StringToLowerMethod =
            typeof(string).GetMethod(nameof(string.ToLower), new Type[] { });

        public static readonly MethodInfo EnumerableContainsMethod = typeof(Enumerable).GetMethods()
            .FirstOrDefault(m =>
                m.IsGenericMethod && m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2);
    }
}
