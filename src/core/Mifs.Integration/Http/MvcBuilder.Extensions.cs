using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace Mifs.Http
{
    public static class MvcBuilder_Extensions
    {
        public static IMvcBuilder AddRazorApplicationPart(this IMvcBuilder builder, Assembly assembly)
        {
            var assemblyLocation = assembly.Location;
            var assemblyName = assembly.GetName().Name;
            var viewsAssemblyLocation = assemblyLocation.Replace($"{assemblyName}.dll", $"{assemblyName}.Views.dll");
            if (File.Exists(viewsAssemblyLocation))
            {
                var viewsAssembly = Assembly.LoadFile(viewsAssemblyLocation);
                builder.AddApplicationPart(viewsAssembly);
            }

            return builder;
        }
    }
}
