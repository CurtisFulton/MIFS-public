using Mifs.Http;

namespace Mifs.Xero
{
    public static class IntegrationWebHostBuilder_Extensions
    {
        public static IIntegrationWebHostBuilder UseXeroOAuth(this IIntegrationWebHostBuilder webBuilder)
        {
            // TODO: Actually implement this
            webBuilder.UseControllersInAssembly<XeroAuthService>();

            return webBuilder;
        }
    }
}
