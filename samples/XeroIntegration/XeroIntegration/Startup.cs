using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mifs.MEX;
using Mifs.Scheduling;
using Mifs.Xero;
using System.Threading.Tasks;
using XeroIntegration.Export;

namespace XeroIntegration
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add the required services for MEX and Xero.
            services.AddMEX(this.Configuration);
            services.AddXero(this.Configuration);

            // Jobs have to be added to the DI container before they can be scheduled
            // services.AddTransient<MEXPurchaseOrderExport>();
            // services.AddTransient<XeroSupplierImport>();
            services.AddTransient<ExampleFakeExport>();
        }

        public async Task Configure(IIntegrationScheduler scheduler)
        {
            await scheduler.ScheduleJob<ExampleFakeExport, MyCustomExportEntity>(new string[] { "0/10 * * * * ?" }, executeOnStartup: true);

            // The following import/exports require a MEX Db connection
            // await scheduler.ScheduleJob<MEXPurchaseOrderExport, PurchaseOrder>(Array.Empty<string>(), executeOnStartup: false);
            // await scheduler.ScheduleJob<XeroSupplierImport, XeroContact>(Array.Empty<string>(), executeOnStartup: false);
        }
    }
}
