using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Mifs.Http
{
    /// <summary>
    /// Helper collection to manage the registered proxy routes.
    /// Mostly used to allow multiple locations to have a shared place to access/modify proxies.
    /// </summary>
    public class ProxyRouteCollection
    {
        public ProxyRouteCollection(ILogger<ProxyRouteCollection> logger)
        {
            this.Logger = logger;
        }

        public record ProxyRoute(string IntegrationName, string ProxyToUrl);

        private IMemoryCache IntegrationProxiesCache { get; } = new MemoryCache(new MemoryCacheOptions());
        private ILogger<ProxyRouteCollection> Logger { get; }

        public void AddServerRoute(string integrationName, string proxyToUrl)
        {
            var integrationProxy = new ProxyRoute(integrationName, proxyToUrl);

            this.IntegrationProxiesCache.Set(integrationName, integrationProxy);
            this.Logger.LogInformation("Proxy for {integrationName} has been set to {proxyUrl}.", integrationName, proxyToUrl);
        }

        public void RemoveServerRoute(string integrationName)
        {
            this.IntegrationProxiesCache.Remove(integrationName);
            this.Logger.LogInformation("Proxy for {integrationName} has been removed.", integrationName);
        }
        
        public ProxyRoute GetIntegrationProxy(string integrationName)
            => this.IntegrationProxiesCache.Get<ProxyRoute>(integrationName);
    }
}
