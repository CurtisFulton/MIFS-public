using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace Mifs.Http
{
    public interface IIntegrationWebHostBuilder
    {
        IIntegrationWebHostBuilder ConfigureMvcBuilder(Action<IMvcBuilder> config);
        IIntegrationWebHostBuilder ConfigureMvcOptions(Action<MvcOptions> options);
    }

    public class IntegrationWebHostBuilder : IIntegrationWebHostBuilder
    {
        public IntegrationWebHostBuilder(IHostBuilder builder)
        {
            this.Builder = builder;

            this.ConfigureDefaultWebHost();
        }

        private IHostBuilder Builder { get; }

        private List<Action<IMvcBuilder>> MvcBuilderActions { get; } = new List<Action<IMvcBuilder>>();
        private List<Action<MvcOptions>> MvcOptionsActions { get; } = new List<Action<MvcOptions>>();

        public IIntegrationWebHostBuilder ConfigureMvcBuilder(Action<IMvcBuilder> configAction)
        {
            this.MvcBuilderActions.Add(configAction);
            return this;
        }

        public IIntegrationWebHostBuilder ConfigureMvcOptions(Action<MvcOptions> optionAction)
        {
            this.MvcOptionsActions.Add(optionAction);
            return this;
        }

        private void ConfigureDefaultWebHost()
        {
            this.Builder.ConfigureWebHostDefaults(webBuilder =>
            {
                // Setup the web host as a kestrel server listening for any IP 
                // Passing the port number as 0 to get a random one assigned.
                webBuilder.UseKestrel(config =>
                {
                    config.ListenAnyIP(0);
                });

                webBuilder.ConfigureServices(services =>
                {
                    // Application Feature proxy is required for the parent server to get the assigned port for this web host.
                    services.TryAddSingleton<ApplicationFeatureProxy>();

                    services.AddRazorPages();
                    var mvcBuilder = services.AddControllers(this.ConfigureMvcOptions);
                    this.ConfigureMvcBuilder(mvcBuilder);
                });

                webBuilder.Configure(this.ConfigureWebHost);
            });
        }

        private void ConfigureMvcBuilder(IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddXmlSerializerFormatters();

            foreach (var mvcBuilderAction in this.MvcBuilderActions)
            {
                mvcBuilderAction?.Invoke(mvcBuilder);
            }
        }

        private void ConfigureMvcOptions(MvcOptions options)
        {
            options.AllowEmptyInputInBodyModelBinding = true;

            
            foreach (var mvcOptionAction in this.MvcOptionsActions)
            {
                mvcOptionAction?.Invoke(options);
            }
        }

        private void ConfigureWebHost(WebHostBuilderContext context, IApplicationBuilder app)
        {
            // The parent proxy server needs to get the port that this web host is given, but that port is only 
            // accesible from ServerFeatures => IServerAddressesFeature. The port is also only populated after
            // configure is called and the server starts.
            // But ServerFeatures is only accessible from IApplicationBuilder which cannot be accessed once the server starts.
            // So we store it in a proxy singleton object that will be accessed by the proxy server to setup the route.
            var applicationFeatureProxy = app.ApplicationServices.GetRequiredService<ApplicationFeatureProxy>();
            applicationFeatureProxy.FeatureCollection = app.ServerFeatures;

            // Controllers don't allow Post requests with no Content-Type header, so if there isn't one we manually add it in.
            // Just fall back to the json formatter, which should result in a null object because there should be no content.
            // TODO: Probably fix this at the proxy level rather than like this.
            //app.Use(async (context, next) =>
            //{
            //    var contentTypeHeader = context.Request.Headers.FirstOrDefault(header => header.Key.Equals("Content-Type", StringComparison.CurrentCultureIgnoreCase));
            //    if (contentTypeHeader.Key is null)
            //    {
            //        context.Request.Headers.Add("Content-Type", "application/json");
            //    }

            //    await next();
            //});

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
