using Microsoft.Extensions.DependencyInjection;

namespace Mifs.Http
{
    public static class IntegrationWebHostBuilder_Extensions
    {
        public static IIntegrationWebHostBuilder UseControllersInAssembly<TAssembly>(this IIntegrationWebHostBuilder webHostBuilder)
        {
            webHostBuilder.ConfigureMvcBuilder(config =>
            {
                config.AddApplicationPart(typeof(TAssembly).Assembly);
            });

            return webHostBuilder;
        }
    }
}
