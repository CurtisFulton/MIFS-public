using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mifs.MEX;
using Mifs.MEX.Domain;
using Mifs.Scheduling;
using Mifs.Xero;
using System;
using System.Threading.Tasks;
using XeroIntegration.Export;
using XeroIntegration.Import;

using XeroContact = Xero.NetStandard.OAuth2.Model.Accounting.Contact;

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

            // Jobs have to be added to the DI container to be used when they are scheduled
            // services.AddTransient<MEXPurchaseOrderExport>();
            // services.AddTransient<XeroSupplierImport>();
            services.AddTransient<ExampleFakeExport>();
        }

        public async Task Configure(IIntegrationScheduler scheduler)
        {
            // Either set some cron schedules or make it execute on startup to test the jobs.
            // The following import/exports require a MEX Db connection
            // await scheduler.ScheduleJob<MEXPurchaseOrderExport, PurchaseOrder>(Array.Empty<string>(), executeOnStartup: false);
            // await scheduler.ScheduleJob<XeroSupplierImport, XeroContact>(Array.Empty<string>(), executeOnStartup: false);
            await scheduler.ScheduleJob<ExampleFakeExport, MyCustomExportEntity>(new string[] { "0/10 * * * * ?" }, executeOnStartup: true);
        }
    }
}
