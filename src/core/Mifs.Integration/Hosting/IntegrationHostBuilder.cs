using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Mifs.Extensions;
using Mifs.Scheduling;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    /// <summary>
    /// Default implementation of the IIntegrationHostBuilder.
    /// Allows for a startup class to be registered.
    /// Uses Quartz as the scheduling framework.
    /// </summary>
    internal class IntegrationHostBuilder : IIntegrationHostBuilder, ISupportsStartup
    {
        public IntegrationHostBuilder(IHostBuilder builder)
        {
            this.Builder = builder;

            this.ConfigureDefaultServices(builder);
        }

        private IHostBuilder Builder { get; }

        public IIntegrationHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureAction)
        {
            this.Builder.ConfigureServices((context, services) =>
            {
                configureAction?.Invoke(context, services);
            });

            return this;
        }

        public IIntegrationHostBuilder UseStartup(Type startupType)
        {
            this.Builder.ConfigureServices((context, services) =>
            {
                this.UseStartup(startupType, context, services);
            });

            return this;
        }

        private void UseStartup(Type startupType, HostBuilderContext context, IServiceCollection services)
        {
            // Using the ActivatorUtilities to provide a limited DI, create an instance of the startup class.
            var instance = ActivatorUtilities.CreateInstance(new HostServiceProvider(context), startupType);

            var startupLoader = new StartupLoader(instance, context.HostingEnvironment.EnvironmentName);
            startupLoader.ConfigureServicesMethodInfo?.Invoke(instance, new object[] { services });

            services.Configure<IntegrationHostServiceOptions>(options =>
            {
                options.ConfigureApplication = async services =>
                {
                    if (startupLoader.ConfigureMethodInfo is null)
                    {
                        return;
                    }

                    // The configure method is called after the Host has been built, so we have access to the IServiceProvider.
                    // Because of this we call the configure method, filling out the parameters using the the IServiceProvider
                    var configureResult = startupLoader.ConfigureMethodInfo.CallMethodWithDI(instance, services);
                    if (configureResult is Task configureTask)
                    {
                        await configureTask;
                    }
                };
            });
        }

        private void ConfigureDefaultServices(IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();

                services.TryAddSingleton<IIntegrationScheduler, DefaultIntegrationScheduler>();
                services.TryAddTransient(typeof(ScheduledProcessJob<,,>));
                services.TryAddTransient(typeof(IFactory<>), typeof(GenericFactory<>));

                this.ConfigureQuartz(context, services);
            });
        }

        private void ConfigureQuartz(HostBuilderContext context, IServiceCollection services)
        {
            services.AddQuartz(quartz =>
            {
                var configuration = context.Configuration;
                var integrationName = configuration.GetValue<string>("Integration:Name");

                quartz.SchedulerName = $"{integrationName} Mifs Scheduler";
                quartz.SchedulerId = "AUTO";
                quartz.UseMicrosoftDependencyInjectionScopedJobFactory();
            });

            // This will actually start/stop the scheduler when the DI Host starts/stops.
            services.AddQuartzHostedService(quartz =>
            {
                quartz.WaitForJobsToComplete = true;
            });

            // We use a job wrapper for basic jobs to stop there being a dependency on Quartz
            // when integrations are being written.
            services.TryAddTransient(typeof(BasicScheduledJobWrapper<>));
        }

        /// <summary>
        /// This only exists so we can use ActivatorUtilities.CreateInstance on the startup class
        /// </summary>
        private class HostServiceProvider : IServiceProvider
        {
            public HostServiceProvider(HostBuilderContext context)
            {
                this.Context = context;
            }

            private HostBuilderContext Context { get; }

            public object? GetService(Type serviceType)
            {
                if (serviceType == typeof(IHostEnvironment))
                {
                    return this.Context.HostingEnvironment;
                }

                if (serviceType == typeof(IConfiguration))
                {
                    return this.Context.Configuration;
                }

                return null;
            }
        }
    }
}
