using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    }
}
