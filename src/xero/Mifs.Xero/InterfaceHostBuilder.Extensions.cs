using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mifs.Authentication;
using Mifs.Xero.Api;

namespace Mifs.Xero
{
    public static class InterfaceHostBuilder_Extensions
    {
        public static IServiceCollection AddXero(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddTransient<IAccountingApiService, AccountingApiService>();
            services.TryAddTransient<ITenantService, XeroTenantService>();
            services.TryAddTransient<IAuthService, XeroAuthService>();

            // TODO: Remove this. I don't think using automapper for this is a good idea
            services.AddAutoMapper(typeof(InterfaceHostBuilder_Extensions).Assembly);

            return services;
        }
    }
}
