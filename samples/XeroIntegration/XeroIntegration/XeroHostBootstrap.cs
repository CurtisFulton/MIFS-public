using Microsoft.Extensions.Hosting;
using Mifs;
using Mifs.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace XeroIntegration
{
    public class XeroHostBootstrap : IHostBootstrap
    {
        public void ConfigureBuilder(IHostBuilder hostBuilder)
        {
            hostBuilder
                .ConfigureIntegrationHostDefaults(integrationBuilder =>
                {
                    integrationBuilder.UseStartup<Startup>();
                });
        }

        public Task ConfigureHost(IHost host, CancellationToken cancellationToken)
        {
            // Do things like migrate Db's. 
            // This is stuff that needs to run before the application actually *starts*, but after it has been built.

            return Task.CompletedTask;
        }
    }
}
