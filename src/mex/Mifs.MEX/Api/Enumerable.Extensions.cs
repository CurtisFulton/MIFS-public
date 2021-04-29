using System;
using System.Collections.Generic;
using System.Linq;

namespace Mifs.MEX.Api
{
    public static class Enumerable_Extensions
    {
        public static string AsMEXServiceOpParameterList<T>(this IEnumerable<T> source)
            => string.Join("", source.Select(x => $"({x})"));

        public static string AsMEXServiceOpParameterList<T>(this IEnumerable<T> source, Func<T, string> itemTransformDelegate)
            => string.Join("", source.Select(itemTransformDelegate));
    }
}
