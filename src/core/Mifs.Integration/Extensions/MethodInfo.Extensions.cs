using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mifs.Extensions
{
    public static class MethodInfo_Extensions
    {
        public static object? CallMethodWithDI(this MethodInfo methodInfo, object instance, IServiceProvider serviceProvider)
        {
            var parameters = new List<object>();
            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                var parameterType = parameterInfo.ParameterType;
                var parameterInstance = serviceProvider.GetRequiredService(parameterType);

                parameters.Add(parameterInstance);
            }

            var result = methodInfo.Invoke(instance, parameters.ToArray());
            return result;
        }
    }
}
