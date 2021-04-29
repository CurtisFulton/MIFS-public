using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mifs.Hosting;
using Mifs.Http.Proxy;

namespace Mifs.Bootstrap
{
    public static class Bootstrapper
    {
        public static IHostBuilder CreateMifsHost(string[] args)
            => Host.CreateDefaultBuilder(args)
                   .ConfigureHostRoot(integrationBuilder =>
                   {
                       // Configure the Host root
                   })
                   .ConfigureWebHostProxy(proxyBuilder =>
                   {
                       // Configure the reverse proxy
                   })

                   // Temp. This will be moved to ConfigureHostRoot/ConfigureWebHostProxy
                   .ConfigureServices(services =>
                   {
                       services.AddSingleton<IntegrationHostManager>();
                       services.AddSingleton<ProxyRouteCollection>();
                       services.AddSingleton<IntegrationFileWatcher>();
                       services.AddSingleton<IntegrationRegistrar>();

                       services.AddSingleton<IIntegrationProvider, DefaultIntegrationProvider>();
                       services.AddSingleton<IIntegrationHostFactory, IntegrationHostFactory>();

                       services.AddHostedService<IntegrationInitializationHostService>();
                   });
    }
}
