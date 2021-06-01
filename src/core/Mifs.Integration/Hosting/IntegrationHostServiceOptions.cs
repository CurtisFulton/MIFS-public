using System;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    /// <summary>
    /// Options used by the IntegrationHostService.
    /// Required to allow the Configure method in the IntegrationStartup class.
    /// </summary>
    internal class IntegrationHostServiceOptions
    {
        public Func<IServiceProvider, Task>? ConfigureApplication { get; set; }
    }
}
