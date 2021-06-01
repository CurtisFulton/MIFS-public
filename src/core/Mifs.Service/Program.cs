using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mifs.Http;
using Mifs.Supervisor;
using Serilog;
using Serilog.Core;
using System.Threading.Tasks;

namespace Mifs.Service
{
    /// <summary>
    /// Real entry point for a Production Mifs application
    /// This sets up the Supervisor which will look for Mifs applications to load
    /// </summary>
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseSerilog(logger: CreateSerilogLogger(), dispose: true)
                       .ConfigureSupervisorHost()
                       .ConfigureSupervisorWebHostDefaults(mvcBuilder =>
                       {
                           mvcBuilder.AddApplicationPart(typeof(Mifs.Supervisor.Dashboard.Controllers.DashboardController).Assembly);
                           mvcBuilder.AddRazorApplicationPart(typeof(Mifs.Supervisor.Dashboard.IndexModel).Assembly);
                       })
                       //.UseWindowsService()
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
