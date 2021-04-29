using System;

namespace Mifs.Hosting
{
    public interface ISupportsStartup
    {
        IIntegrationHostBuilder UseStartup(Type startupType);
    }
}
