using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mifs.Hosting;
using Mifs.Http;

namespace Mifs.Bootstrap
{
    internal class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IntegrationHostManager>();
            services.AddSingleton<ProxyRouteCollection>();
            services.AddSingleton<IntegrationFileWatcher>();
            services.AddSingleton<IntegrationRegistrar>();

            services.AddSingleton<IProxyRouter, IntegrationProxyRouter>();
            services.AddSingleton<IIntegrationProvider, DefaultIntegrationProvider>();
            services.AddSingleton<IIntegrationHostFactory, IntegrationHostFactory>();

            services.AddHttpProxy();
            services.AddRazorPages();
            services.AddControllers();

            services.AddHostedService<IntegrationInitializationHostService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //var mifsEmbeddedFileProvider = new EmbeddedFileProvider(typeof(IIntegrationInitialization).Assembly, "Mifs.wwwroot");

            //app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = mifsEmbeddedFileProvider
            //});

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                // All remaining routes are to be proxied to another site if one exists.
                var integrationProxy = endpoints.ServiceProvider.GetRequiredService<IProxyRouter>();
                endpoints.MapFallback(async httpContext => await integrationProxy.HandleProxy(httpContext));
            });
        }
    }
}
