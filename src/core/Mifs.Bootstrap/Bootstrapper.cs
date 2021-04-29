using Microsoft.Extensions.Hosting;
using Mifs.Hosting;
using Mifs.Http.Proxy;
using System.Reflection;

namespace Mifs.Bootstrap
{
    public static class Bootstrapper
    {
        public static IHostBuilder CreateMifsHost(string[] args)
            => Host.CreateDefaultBuilder(args)
                   .ConfigureHostRoot(integrationBuilder =>
                   {
                       // Configure the Host root
                   })
                   .ConfigureWebHostProxy(proxyBuilder =>
                   {
                       // Configure the reverse proxy
                   });
    }
}
