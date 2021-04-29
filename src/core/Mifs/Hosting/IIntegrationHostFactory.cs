using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mifs.Http;
using Mifs.Http.Proxy;
using Serilog;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    public interface IIntegrationHostFactory
    {
        Task<IHost?> CreateAndStart(IHostBootstrap hostBootstrapper,
                                    IConfiguration configurationFiles,
                                    CancellationToken cancellationToken = default);
    }

    public class IntegrationHostFactory : IIntegrationHostFactory
    {
        private ProxyRouteCollection ProxyRouteCollection { get; }
        private ILogger<IntegrationHostFactory> Logger { get; }

        public IntegrationHostFactory(ProxyRouteCollection proxyRouteCollection,
                                      ILogger<IntegrationHostFactory> logger)
        {
            // Not a huge fan of having the host factory configuring the proxy routes...
            // Might change this later
            this.ProxyRouteCollection = proxyRouteCollection;
            this.Logger = logger;
        }

        public async Task<IHost?> CreateAndStart(IHostBootstrap hostBootstrapper, 
                                                 IConfiguration configuration,
                                                 CancellationToken cancellationToken = default)
        {
            var integrationName = configuration.GetValue<string?>("Integration:Name");
            if (integrationName is null)
            {
                this.Logger.LogError("Failed to start an integration because there was no Name specified in the configuration. Make sure appsettings.json contains the following configuration 'Integration:Name'");
                return null;
            }

            var logger = this.CreateSerilogLoggerForHost(integrationName);

            // Setup a default host builder with serilog attached
            var hostBuilder = Host.CreateDefaultBuilder()
                                  .UseSerilog(logger: logger, dispose: true);

            // Setup all the configuration files for the host
            this.SetupIntegrationConfiguration(configuration, hostBuilder);

            // Pass the host builder to the initializer to actually configure it.
            hostBootstrapper.ConfigureBuilder(hostBuilder);

            // Now actually build the host and allow the integration to do any configuration before actually starting the host
            // This will likely be things like migrating Db's.
            var host = hostBuilder.Build();

            await hostBootstrapper.ConfigureHost(host, cancellationToken);

            // Start the host
            await host.StartAsync(cancellationToken);

            // If an ApplicationFeatureProxy exists it means a Kestrel server exists for this host.
            // So we want to add it to the ProxyRouteCollection.
            var applicationFeatures = host.Services.GetService<ApplicationFeatureProxy>();
            if (applicationFeatures is not null)
            {
                this.SetupIntegrationServerProxy(integrationName, applicationFeatures);
            }

            this.Logger.LogInformation("{integrationName} Host created.", integrationName);
            return host;
        }

        private Serilog.ILogger CreateSerilogLoggerForHost(string integrationName)
        {
            // TODO: Clean this up
            return new LoggerConfiguration()
                                .MinimumLevel.Information()
                                .WriteTo.Console()
                                .Enrich.WithProperty("IntegrationName", integrationName)
                                .Enrich.FromLogContext()
                                .CreateLogger();
        }

        private void SetupIntegrationServerProxy(string integrationName, ApplicationFeatureProxy applicationFeatures)
        {
            var serverAdress = applicationFeatures.FeatureCollection
                                                  ?.Get<IServerAddressesFeature>()
                                                  ?.Addresses
                                                  ?.FirstOrDefault();
            if (serverAdress is null)
            {
                return;
            }

            var uri = new Uri(serverAdress);
            var port = uri.Port;

            var proxyToUrl = $"http://localhost:{port}/";
            this.Logger.LogInformation("Kestrel server registration found for Integration {integrationName}.", integrationName);
            this.ProxyRouteCollection.AddServerRoute(integrationName, proxyToUrl);
        }

        private void SetupIntegrationConfiguration(IConfiguration configuration, IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureAppConfiguration(builder =>
            {
                builder.Sources.Clear();

                // Add the provided configuration plus environment variables
                builder.AddConfiguration(configuration);
                builder.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                // Setup the IntegrationConfiguration as an IOptions<> object in the Host
                var configuration = context.Configuration;
                var integrationConfiguration = configuration.GetSection("Integration");

                services.Configure<IntegrationConfiguration>(integrationConfiguration);
            });
        }
    }
}
