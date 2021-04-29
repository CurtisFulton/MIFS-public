using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Mifs.Bootstrap;
using Mifs.Hosting;
using Mifs.Http;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Mifs.Service
{
    public static class Program
    {
        public static Task Main(string[] args)
        {
            return Bootstrapper.CreateMifsHost(args)
                               .UseWindowsService()
                               .Build()
                               .RunAsync();
        }
    }
}
