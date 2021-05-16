using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Mifs.Http
{
    public static class HostBuilder_Extensions
    {
        public static IHostBuilder ConfigureIntegrationWebHost(this IHostBuilder builder, Action<IIntegrationWebBuilder>? configureWebProxy = null)
        {

            return builder;
        }

        public static IHostBuilder ConfigureIntegrationProxyDefaults(this IHostBuilder builder, Action<IMvcBuilder> configureMvc)
        {
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    services.AddIntegrationProxy();

                    var mvcBuilder = services.AddControllers();
                    services.AddRazorPages();

                    configureMvc?.Invoke(mvcBuilder);
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                        endpoints.MapControllers();

                        endpoints.MapIntegrationFallback();
                    });
                });

                if (OperatingSystem.IsWindows())
                {
                    webBuilder.UseHttpSys(config =>
                    {
                        config.UrlPrefixes.Add("http://*:80/Mifs/");
                    });
                }
                else
                {
                    // TODO. Setup kestrel server?
                }
            });

            return builder;
        }

        public static IServiceCollection AddIntegrationProxy(this IServiceCollection services)
        {
            services.TryAddSingleton<ProxyRouteCollection>();
            services.TryAddSingleton<IProxyRouter, HeaderIntegrationProxyRouter>();
            services.AddHttpProxy();

            return services;
        }
    }
}
