using Microsoft.Extensions.Hosting;
using Mifs.Hosting;
using Mifs.Http;
using Serilog;
using Serilog.Core;
using System.Threading.Tasks;

namespace Mifs.Service
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseSerilog(logger: CreateSerilogLogger(), dispose: true)
                       .ConfigureRootIntegrationHost()
                       .ConfigureIntegrationProxyDefaults(mvcBuilder =>
                       {
                           mvcBuilder.AddRazorApplicationPart(typeof(Mifs.Dashboard.IndexModel).Assembly);
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
