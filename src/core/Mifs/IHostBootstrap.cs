using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs
{
    public interface IHostBootstrap
    {
        void ConfigureBuilder(IHostBuilder hostBuilder);
        Task ConfigureHost(IHost host, CancellationToken cancellationToken);
    }
}
