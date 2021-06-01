using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    /// <summary>
    /// Hosted service used to call the configure method on the IntegrationStartup class.
    /// It uses the IntegrationHostServiceOptions to store the delegate to call.
    /// </summary>
    internal class IntegrationHostService : IHostedService
    {
        public IntegrationHostService(IOptions<IntegrationHostServiceOptions> options, IServiceProvider services)
        {
            this.Options = options.Value;
            this.Services = services;
        }

        private IntegrationHostServiceOptions Options { get; }
        private IServiceProvider Services { get; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (this.Options.ConfigureApplication is null)
            {
                return;
            }

            using var scope = this.Services.CreateScope();
            await this.Options.ConfigureApplication.Invoke(scope.ServiceProvider);
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}
