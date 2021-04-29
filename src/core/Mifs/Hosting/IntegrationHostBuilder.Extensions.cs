using System;

namespace Mifs.Hosting
{
    public static class IntegrationHostBuilder_Extensions
    {
        public static IIntegrationHostBuilder UseStartup<TStartup>(this IIntegrationHostBuilder builder)
            => builder.UseStartup(typeof(TStartup));

        public static IIntegrationHostBuilder UseStartup(this IIntegrationHostBuilder builder, Type startupType)
        {
            if (builder is ISupportsStartup supportsStartup)
            {
                supportsStartup.UseStartup(startupType);
            }

            return builder;
        }
    }
}
