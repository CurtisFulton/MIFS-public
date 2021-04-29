using Microsoft.Extensions.DependencyInjection;
using Mifs.Http;

namespace Mifs.Xero
{
    public static class IntegrationWebHostBuilder_Extensions
    {
        public static IIntegrationWebHostBuilder UseXeroOAuth(this IIntegrationWebHostBuilder webBuilder)
        {
            webBuilder.UseControllersInAssembly<XeroAuthService>();

            return webBuilder;
        }
    }
}
