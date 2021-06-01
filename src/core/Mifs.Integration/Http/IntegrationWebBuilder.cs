using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mifs.Http
{
    public interface IIntegrationWebBuilder
    {
        IIntegrationWebBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureServices);
        IIntegrationWebBuilder ConfigureMvcBuilder(Action<IMvcBuilder> config);
        IIntegrationWebBuilder ConfigureMvcOptions(Action<MvcOptions> options);
        IIntegrationWebBuilder ConfigureEndpoints(Action<IEndpointRouteBuilder> options);
    }

    internal class IntegrationWebBuilder : IIntegrationWebBuilder
    {
        public IntegrationWebBuilder(IHostBuilder builder)
        {
            this.Builder = builder;

            this.CreateDefaultWebHost();
        }

        private IHostBuilder Builder { get; }
        private List<Action<IMvcBuilder>> MvcBuilderActions { get; } = new List<Action<IMvcBuilder>>();
        private List<Action<MvcOptions>> MvcOptionsActions { get; } = new List<Action<MvcOptions>>();
        private List<Action<IEndpointRouteBuilder>> EndpointsBuilderActions { get; } = new List<Action<IEndpointRouteBuilder>>();

        public IIntegrationWebBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureServices)
        {
            this.Builder.ConfigureServices(configureServices);
            return this;
        }

        public IIntegrationWebBuilder ConfigureMvcBuilder(Action<IMvcBuilder> config)
        {
            this.MvcBuilderActions.Add(config);
            return this;
        }

        public IIntegrationWebBuilder ConfigureMvcOptions(Action<MvcOptions> options)
        {
            this.MvcOptionsActions.Add(options);
            return this;
        }

        public IIntegrationWebBuilder ConfigureEndpoints(Action<IEndpointRouteBuilder> config)
        {
            this.EndpointsBuilderActions.Add(config);
            return this;
        }

        private void CreateDefaultWebHost()
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
                    services.TryAddSingleton<ApplicationFeatureSurrogate>();

                    if (this.MvcBuilderActions.Any() || this.MvcOptionsActions.Any())
                    {
                        var mvcBuilder = services.AddControllers(this.ConfigureMvcOptions);
                        this.ConfigureMvcBuilder(mvcBuilder);
                    }
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
            var applicationFeatureSurrogate = app.ApplicationServices.GetRequiredService<ApplicationFeatureSurrogate>();
            applicationFeatureSurrogate.FeatureCollection = app.ServerFeatures;

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                if (this.MvcBuilderActions.Any())
                {
                    endpoints.MapControllers();
                }

                foreach (var endpointBuilderAction in this.EndpointsBuilderActions)
                {
                    endpointBuilderAction.Invoke(endpoints);
                }
            });
        }
    }
}
