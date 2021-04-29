using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Mifs.MEX.Api;
using Mifs.MEX.Authentication;
using Mifs.MEX.Logging;
using Mifs.MEX.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mifs.MEX
{
    public static class IntegrationHostBuilder_Extensions
    {
        public static IServiceCollection AddMEX(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddTransient<IPurchaseOrderApprovalService, PurchaseOrderApprovalService>();
            services.TryAddScoped(typeof(IProcessedEntityLogger<>), typeof(ProcessedEntityLogger<>));

            services.TryAddSingleton<IMEXHttpService, MEXHttpService>();
            services.TryAddSingleton<IMEXAuthenticationService, MEXAuthenticationService>();

            services.TryAddSingleton<IServiceOpService, ServiceOpService>();
            services.TryAddSingleton<ISystemOptionService, SystemOptionService>();

            services.AddTransient<PurchaseOrderApi>();
            services.AddTransient<WorkOrderApi>();
            services.AddTransient<AssetReadingApi>();

            services.Configure<MEXConfiguration>(configuration.GetSection("MEX"));

            AddMEXDbContext(services, configuration);
            AddMEXHttpClient(services, configuration);

            return services;
        }

        private static void AddMEXDbContext(IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddTransient<IMEXDbContext, MEXDbContext>();

            services.AddDbContextFactory<MEXDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("MEX");
                options.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.CommandTimeout(120);
                });
            });
        }

        private static void AddMEXHttpClient(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(MEXHttpService.HttpClientName, ConfigureBaseMEXHttpClient)
                    .AddHttpMessageHandler<MEXAccessTokenDelegatingHandler>()
                    .AddHttpMessageHandler<MEXDateTimeHeadersDelegatingHandler>();

            services.AddHttpClient(MEXAuthenticationService.HttpClientName, ConfigureAuthorizationMEXHttpClient)
                    .AddHttpMessageHandler<MEXDateTimeHeadersDelegatingHandler>();

            services.AddTransient<MEXAccessTokenDelegatingHandler>();
            services.AddTransient<MEXDateTimeHeadersDelegatingHandler>();

            void ConfigureBaseMEXHttpClient(HttpClient clientConfig)
            {
                var baseAddress = configuration.GetValue<string>("MEX:BaseUrl");
                clientConfig.BaseAddress = new Uri(baseAddress);

                clientConfig.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            void ConfigureAuthorizationMEXHttpClient(HttpClient clientConfig)
            {
                var baseAddress = configuration.GetValue<string>("MEX:BaseUrl");
                if (!baseAddress.EndsWith("/"))
                {
                    baseAddress += "/";
                }

                var authenticationAddress = baseAddress + "API/Authentication/";
                clientConfig.BaseAddress = new Uri(authenticationAddress);

                clientConfig.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }
    }
}
