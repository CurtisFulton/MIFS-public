using Microsoft.Extensions.Hosting;
using Mifs.Supervisor;
using Mifs.Supervisor.Dashboard;
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
                       .ConfigureSupervisorHost()
                       .ConfigureSupervisorWebHostDefaults(mvcBuilder =>
                       {
                           mvcBuilder.AddMifsDashboard();
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
