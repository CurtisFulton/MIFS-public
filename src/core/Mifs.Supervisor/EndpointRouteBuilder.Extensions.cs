using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Mifs.Supervisor
{
    public static class EndpointRouteBuilder_Extensions
    {
        public static IEndpointRouteBuilder MapIntegrationFallback(this IEndpointRouteBuilder endpoints)
        {
            var integrationProxy = endpoints.ServiceProvider.GetRequiredService<IProxyRouter>();
            endpoints.MapFallback(async httpContext => await integrationProxy.HandleProxy(httpContext));

            return endpoints;
        }
    }
}
