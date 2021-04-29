using Microsoft.Extensions.Hosting;
using System;

namespace Mifs.Http.Proxy
{
    public interface IWebProxyBuilder { }

    public static class HostBuilder_Extensions
    {
        public static IHostBuilder ConfigureWebHostProxy(this IHostBuilder builder, Action<IWebProxyBuilder> configureWebProxy)
        {
            // TODO:

            return builder;
        }
    }
}
