using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mifs.Hosting;
using Mifs.Http;
using Mifs.Service;
using Serilog;
using Serilog.Core;
using System.Threading.Tasks;

namespace XeroIntegration.Bootstrap
{
    /// <summary>
    /// In the production situation, there is a windows service that looks recursively through a folder structure for integrations to load.
    /// That would make for annoying debugging, so instead this simple console application acts as the entry point for the integration during debug.
    /// The Mifs:IntegrationPath in Appsetting.json points to the bin folder of the main project to load it.
    /// </summary>
    internal static class Program
    {
        private static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseSerilog(logger: CreateSerilogLogger(), dispose: true)
                       .ConfigureIntegrationProxyDefaults(mvcBuilder =>
                       {
                           // Add the dashboard
                           mvcBuilder.AddRazorApplicationPart(typeof(Mifs.Dashboard.IndexModel).Assembly);
                       })
                       .ConfigureRootIntegrationHost()
                       .ConfigureServices(services =>
                       {
                           services.AddHostedService<ApplicationPartsLogger>();
                       })
                       .RunConsoleAsync();

        }

        private static Logger CreateSerilogLogger()
            => new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .Enrich.WithProperty("Application", "Mifs.Service")
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProcessId()
                .CreateLogger();
    }
}
