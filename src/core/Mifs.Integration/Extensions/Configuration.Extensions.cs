using Microsoft.Extensions.Configuration;

namespace Mifs.Extensions
{
    public static class Configuration_Extensions
    {
        public static TValue Get<TValue>(this IConfiguration configuration, string key)
        {
            var configSection = configuration;

            if (!key.IsNullOrWhiteSpace())
            {
                configSection = configuration.GetSection(key);
            }

            return configSection.Get<TValue>();
        }
    }
}
