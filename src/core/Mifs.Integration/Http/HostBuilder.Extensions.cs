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
        /// <summary>
        /// Initializes an Integration Web Host builder.
        /// This will setup a default ASP.Net server that listens on a random port. 
        /// </summary>
        /// <param name="builder">IHostBuilder to add the server to</param>
        /// <param name="configure">Configure callback. Allows for adding additional functionality to the web host, including configuring MVC.</param>
        /// <returns>The same IHostBuilder passed in to allow for chained calls</returns>
        public static IHostBuilder ConfigureIntegrationWebHostDefaults(this IHostBuilder builder, Action<IIntegrationWebHostBuilder>? configure = null)
        {
            _ = builder ?? throw new NullReferenceException(nameof(builder));
            _ = configure ?? throw new NullReferenceException(nameof(configure));

            var webBuilder = new IntegrationWebHostBuilder(builder);

            configure?.Invoke(webBuilder);

            return builder;
        }

    }
}
