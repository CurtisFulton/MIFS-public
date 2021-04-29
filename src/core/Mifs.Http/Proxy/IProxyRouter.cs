using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.ReverseProxy.Service.Proxy;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mifs.Http.Proxy
{
    public interface IProxyRouter
    {
        Task HandleProxy(HttpContext httpContext);
    }

    /// <summary>
    /// Proxy router that sits on the root Host to proxy requests from a publicly accessible
    /// endpoint, to the child Host kestrel servers.
    /// By default it will try listening on port 80, with a path of /MEXIntegration.
    /// The integration to proxy to is determined by the 'Integration-Name header'
    /// </summary>
    public class IntegrationProxyRouter : IProxyRouter
    {
        private static string IntegrationNameHeader { get; } = "Integration-Name";

        public IntegrationProxyRouter(IHttpProxy httpProxy,
                                      ProxyRouteCollection proxyRouteCollection,
                                      ILogger<IntegrationProxyRouter> logger)
        {
            this.HttpProxy = httpProxy;
            this.ProxyRouteCollection = proxyRouteCollection;
            this.Logger = logger;
        }

        private IHttpProxy HttpProxy { get; }
        private ProxyRouteCollection ProxyRouteCollection { get; }
        private ILogger<IntegrationProxyRouter> Logger { get; }

        private HttpMessageInvoker HttpClient { get; } = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false
        });

        /// <summary>
        /// Takes in the HttpContext for a request and tries routing it to a registered and running integration.
        /// It looks in the Headers for the integration name, and tries to find a registered integration with that name.
        /// </summary>
        /// <param name="httpContext">HttpContext to route</param>
        /// <returns>A Task representing when the request has finished</returns>
        public async Task HandleProxy(HttpContext httpContext)
        {
            var requestUrl = httpContext.Request.GetDisplayUrl();
            this.Logger.LogInformation("Attempting to proxy request {httpMethod} {requestUrl}. ", httpContext.Request.Method, requestUrl);

            var integrationName = this.GetIntegrationNameFromRequest(httpContext.Request);
            if (string.IsNullOrWhiteSpace(integrationName))
            {
                this.Logger.LogWarning("Could not get an integration name from the request with path {requestUrl}.", requestUrl);

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsync($"Could not get the intended integration name from the request. Have you set the {IntegrationNameHeader} header correctly?");
                return;
            }

            var integrationProxy = this.ProxyRouteCollection.GetIntegrationProxy(integrationName);
            if (integrationProxy?.ProxyToUrl is null)
            {
                this.Logger.LogWarning("Could find any running integrations with the name {integrationName}.", integrationName);

                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await httpContext.Response.WriteAsync($"Could find an integration with the name {integrationName}.");
                return;
            }

            await this.HttpProxy.ProxyAsync(httpContext, integrationProxy.ProxyToUrl, this.HttpClient);
        }

        private string GetIntegrationNameFromRequest(HttpRequest request)
            => request.Headers.FirstOrDefault(x => x.Key.Equals(IntegrationProxyRouter.IntegrationNameHeader, StringComparison.CurrentCultureIgnoreCase))
                              .Value;

    }
}
