using Microsoft.AspNetCore.Mvc.RazorPages;
using Mifs.Hosting;
using System.Collections.Generic;

namespace Mifs.Dashboard
{
    public record IntegrationVM(string Name, HostStatus Status, string Directory);

    public class IndexModel : PageModel
    {
        public ICollection<IntegrationVM> Integrations { get; set; } = new List<IntegrationVM>();

        public IndexModel(IntegrationRegistrar integrationRegistrar,
                          IntegrationHostManager integrationHostManager)
        {
            this.IntegrationRegistrar = integrationRegistrar;
            this.IntegrationHostManager = integrationHostManager;
        }

        private IntegrationRegistrar IntegrationRegistrar { get; }
        private IntegrationHostManager IntegrationHostManager { get; }

        public void OnGet()
        {
            this.Integrations.Clear();

            foreach (var registration in this.IntegrationRegistrar.GetIntegrations())
            {
                var integrationHostStatus = this.IntegrationHostManager.GetIntegrationHostStatus(registration.Name);
                var integration = new IntegrationVM(registration.Name, integrationHostStatus ?? HostStatus.Stopped, registration.Directory);
                this.Integrations.Add(integration);
            }
        }
    }
}
