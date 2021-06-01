using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Mifs.Supervisor
{
    /// <summary>
    /// Helper collection to manage the registered proxy routes.
    /// Used to allow multiple classes inject a shared singleton instance to access/modify proxies.
    /// </summary>
    public class ProxyRouteCollection
    {
        public ProxyRouteCollection(ILogger<ProxyRouteCollection> logger)
        {
            this.Logger = logger;
        }

        public record ProxyRoute(string IntegrationName, string ProxyToUrl);

        private ConcurrentDictionary<string, ProxyRoute> IntegrationProxiesCache { get; } = new ConcurrentDictionary<string, ProxyRoute>();
        private ILogger<ProxyRouteCollection> Logger { get; }

        public void AddServerRoute(string integrationName, string proxyToUrl)
        {
            var integrationProxy = new ProxyRoute(integrationName, proxyToUrl);

            this.IntegrationProxiesCache[integrationName] = integrationProxy;
            this.Logger.LogInformation("Proxy for {integrationName} has been set to {proxyUrl}.", integrationName, proxyToUrl);
        }

        public void RemoveServerRoute(string integrationName)
        {
            this.IntegrationProxiesCache.TryRemove(integrationName, out var _);
            this.Logger.LogInformation("Proxy for {integrationName} has been removed.", integrationName);
        }
        
        public ProxyRoute? GetIntegrationProxy(string integrationName)
        {
            if (this.IntegrationProxiesCache.TryGetValue(integrationName, out var proxyValue))
            {
                return proxyValue;
            }

            return null;
        }
    }
}
