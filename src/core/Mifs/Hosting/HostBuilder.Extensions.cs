using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Mifs.Http;
using System;

namespace Mifs.Hosting
{
    public static class HostBuilder_Extensions
    {
        /// <summary>
        /// Initializes a default Integration Host Builder.
        /// This will setup default services required for Mifs such as the scheduling framework.
        /// More dependencies can be added using the configure delegate.
        /// </summary>
        /// <param name="builder">IHostBuilder to add Integration defaults to</param>
        /// <param name="configure">Configure callback. Allows for adding additional functionality to the integration</param>
        /// <returns>The same IHostBuilder passed in to allow for chained calls</returns>
        public static IHostBuilder ConfigureIntegrationHostDefaults(this IHostBuilder builder, Action<IIntegrationHostBuilder> configure)
        {
            _ = builder ?? throw new NullReferenceException(nameof(builder));
            _ = configure ?? throw new NullReferenceException(nameof(configure));

            var integrationHostBuilder = new IntegrationHostBuilder(builder);
            configure.Invoke(integrationHostBuilder);
            builder.ConfigureServices((_, services) =>
            {
                services.AddHostedService<IntegrationHostService>();
            });

            return builder;
        }

        public static IHostBuilder ConfigureRootIntegrationHost(this IHostBuilder builder)
        {
            _ = builder ?? throw new NullReferenceException(nameof(builder));

            builder.ConfigureServices(services =>
            {
                services.TryAddSingleton<IntegrationHostManager>();
                services.TryAddSingleton<IntegrationFileWatcher>();
                services.TryAddSingleton<IntegrationRegistrar>();
                services.TryAddSingleton<IntegrationInitializationHostService>();

                services.TryAddSingleton<IIntegrationProvider, DefaultIntegrationProvider>();
                services.TryAddSingleton<IIntegrationHostFactory, IntegrationHostFactory>();

                services.TryAddSingleton<ProxyRouteCollection>();
                services.TryAddSingleton<IProxyRouter, HeaderIntegrationProxyRouter>();
            });

            return builder;
        }

        /// <summary>
        /// Initializes an Integration Web Host builder.
        /// This will setup a default ASP.Net server that listens on a random port. 
        /// </summary>
        /// <param name="builder">IHostBuilder to add the server to</param>
        /// <param name="configure">Configure callback. Allows for adding additional functionality to the web host, including configuring MVC.</param>
        /// <returns>The same IHostBuilder passed in to allow for chained calls</returns>
        public static IHostBuilder ConfigureIntegrationWebHostDefaults(this IHostBuilder builder, Action<IIntegrationWebHostBuilder> configure)
        {
            _ = builder ?? throw new NullReferenceException(nameof(builder));
            _ = configure ?? throw new NullReferenceException(nameof(configure));

            var webBuilder = new IntegrationWebHostBuilder(builder);

            configure?.Invoke(webBuilder);

            return builder;
        }
    }
}
