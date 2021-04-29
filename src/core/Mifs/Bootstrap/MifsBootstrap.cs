using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using System;

namespace Mifs.Bootstrap
{
    public static class MifsBootstrap
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog(logger: CreateSerilogLogger(), dispose: true)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    if (OperatingSystem.IsWindows())
                    {
                        webBuilder.UseHttpSys(config =>
                        {
                            config.UrlPrefixes.Add("http://*:80/Mifs/");
                        });
                    }
                    else
                    {
                        // TODO. Setup kestrel server?
                    }
                });

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
