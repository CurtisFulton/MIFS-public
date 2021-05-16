using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Service
{
    public class ApplicationPartsLogger : IHostedService
    {
        private readonly ApplicationPartManager _partManager;

        public ApplicationPartsLogger(ApplicationPartManager partManager)
        {
            _partManager = partManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Get the names of all the application parts. This is the short assembly name for AssemblyParts
            var applicationParts = _partManager.ApplicationParts.Select(x => x.Name);

            // Create a controller feature, and populate it from the application parts
            var controllerFeature = new ViewsFeature();
            _partManager.PopulateFeature(controllerFeature);

            // Get the names of all of the controllers
            var controllers = controllerFeature.ViewDescriptors.Select(x => x.Type);

            // Log the application parts and controllers
            var parts = string.Join(", ", applicationParts);
            var controllersTest = string.Join(", ", controllers);

            return Task.CompletedTask;
        }

        // Required by the interface
        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
