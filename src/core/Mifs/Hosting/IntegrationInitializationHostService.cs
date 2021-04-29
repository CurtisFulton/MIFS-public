using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    /// <summary>
    /// Host service for the root Host.
    /// Uses the IIntegrationProvider to register each of the integrations and start up their Hosts.
    /// </summary>
    public class IntegrationInitializationHostService : IHostedService
    {
        public IntegrationInitializationHostService(IIntegrationProvider integrationProvider,
                                                    IntegrationRegistrar integrationRegistrar,
                                                    IntegrationHostManager integrationManager)
        {
            this.IntegrationProvider = integrationProvider;
            this.IntegrationRegistrar = integrationRegistrar;
            this.IntegrationHostManager = integrationManager;
        }

        private IIntegrationProvider IntegrationProvider { get; }
        private IntegrationRegistrar IntegrationRegistrar { get; }
        private IntegrationHostManager IntegrationHostManager { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var integrationRegistrations = this.IntegrationProvider.GetIntegrations();

            await foreach (var integrationRegistration in integrationRegistrations.WithCancellation(cancellationToken))
            {
                // Integrations have to registered before a host can be spun up for them
                // So register then try starting it.
                this.IntegrationRegistrar.Register(integrationRegistration);
                await this.IntegrationHostManager.TryStartHost(integrationRegistration.Name, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
