using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Mifs.Hosting
{
    public interface IIntegrationHostBuilder
    {
        IIntegrationHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureAction);
    }
}
