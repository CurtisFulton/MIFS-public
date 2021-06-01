using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;

namespace Mifs.Supervisor
{
    public static class HostBuilder_Extensions
    {
        public static IServiceCollection AddIntegrationProxy(this IServiceCollection services)
        {
            services.TryAddSingleton<ProxyRouteCollection>();
            services.TryAddSingleton<IProxyRouter, HeaderIntegrationProxyRouter>();
            services.AddHttpProxy();

            return services;
        }

        public static IHostBuilder ConfigureSupervisorHost(this IHostBuilder builder)
        {
            _ = builder ?? throw new NullReferenceException(nameof(builder));

            builder.ConfigureServices(services =>
            {
                services.TryAddSingleton<IntegrationHostManager>();
                services.TryAddSingleton<IntegrationFileWatcher>();
                services.TryAddSingleton<IntegrationRegistrar>();

                services.TryAddSingleton<IIntegrationProvider, DefaultIntegrationProvider>();
                services.TryAddSingleton<IIntegrationHostFactory, IntegrationHostFactory>();

                services.TryAddSingleton<ProxyRouteCollection>();
                services.TryAddSingleton<IProxyRouter, HeaderIntegrationProxyRouter>();

                // This is the hosted service that actually searches for any integrations and starts them up.
                services.AddHostedService<IntegrationInitializationHostService>();
            });

            return builder;
        }

        public static IHostBuilder ConfigureSupervisorWebHostDefaults(this IHostBuilder builder, Action<IMvcBuilder>? configureMvc = null)
        {
            builder.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices(services =>
                {
                    services.AddIntegrationProxy();

                    var mvcBuilder = services.AddControllersWithViews();
                    services.AddRazorPages();

                    configureMvc?.Invoke(mvcBuilder);
                });

                webBuilder.Configure((context, app) =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                        endpoints.MapControllers();

                        //endpoints.MapIntegrationFallback();
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
    }
}
