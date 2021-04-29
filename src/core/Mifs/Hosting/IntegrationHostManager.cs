using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mifs.Http.Proxy;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    public enum HostStatus
    {
        Started,
        Stopped,
        Disabled,
        Errored
    }

    /// <summary>
    /// Manager that keeps track of the IHost's for each of the integrations.
    /// It is responsible for cleaning up the Host's resources when starting/stopping integrations. 
    /// </summary>
    public sealed class IntegrationHostManager : IDisposable
    {
        public IntegrationHostManager(ProxyRouteCollection proxyRouteCollection,
                                      IntegrationRegistrar integrationRegistrar,
                                      IIntegrationHostFactory integrationHostFactory,
                                      ILogger<IntegrationHostManager> logger)
        {
            this.ProxyRouteCollection = proxyRouteCollection;
            this.IntegrationRegistrar = integrationRegistrar;
            this.IntegrationHostFactory = integrationHostFactory;
            this.Logger = logger;

            // Listen for the Deregister and file modified events from the registrar.
            // This allows us to clean up or restart the integrations when required.
            this.IntegrationRegistrar.OnIntegrationDeregistered += this.IntegrationRegistrar_OnIntegrationDeregistered;
            this.IntegrationRegistrar.OnIntegrationFilesModified += this.IntegrationRegistrar_OnIntegrationFilesModified;
        }

        private IMemoryCache IntegrationHostCache { get; } = new MemoryCache(new MemoryCacheOptions());
        private ProxyRouteCollection ProxyRouteCollection { get; }
        private IntegrationRegistrar IntegrationRegistrar { get; }
        private IIntegrationHostFactory IntegrationHostFactory { get; }
        private ILogger<IntegrationHostManager> Logger { get; }

        public bool IsHostRunning(string integrationName)
            => this.IsHostRunning(this.IntegrationHostCache.Get<IntegrationHostData>(integrationName));

        private bool IsHostRunning(IntegrationHostData hostData)
            => hostData?.Status == HostStatus.Started;

        /// <summary>
        /// Attempts to start the host.
        /// Will do validation to check that it registered, valid, and not already running.
        /// </summary>
        /// <param name="integrationName">Name of the integration to start. The integration must be registed in the IntegrationRegistrar to be startable</param>
        /// <param name="cancellationToken">Cancellation token to stop the startup</param>
        public async Task TryStartHost(string integrationName, CancellationToken cancellationToken = default)
        {
            this.Logger.LogInformation("Attempting to start {integrationName}...", integrationName);

            if (!this.IntegrationRegistrar.TryGetRegistration(integrationName, out var integrationRegistration))
            {
                this.Logger.LogWarning("Failed to start {integrationName}. No integration with that name has been registered.", integrationName);
                return;
            }

            // If the host is running, we don't need to do anything
            if (this.IsHostRunning(integrationName))
            {
                this.Logger.LogInformation("{integrationName} not started because it is already started.", integrationName);
                return;
            }

            // If the host isn't running, we make sure there is no entry for it in the IntegrationHostCache
            // This will allow it to be created in the next step.
            this.IntegrationHostCache.Remove(integrationName);

            // We use GetOrCreateAsync instead of Set because it's possible that 2 threads could call TryStartHost at the same time
            // If they both made it to this point and use Set, we would be creating 2 hosts and instantly disposing of 1 of them which would be wasteful.
            // Instead we allow the IMemoryCache to deal with threading issues.
            await this.IntegrationHostCache.GetOrCreateAsync(integrationName, entry => this.CreateAndStartIntegrationHost(entry, integrationRegistration, cancellationToken));
        }

        /// <summary>
        /// Stops the Host if there is one currently running.
        /// This will dispose of any resources the Host uses.
        /// </summary>
        /// <param name="integrationName">Name of the integration to stop.</param>
        public async Task StopHost(string integrationName)
        {
            this.Logger.LogInformation("Attempting to stop {integrationName}...", integrationName);

            if (!this.IntegrationHostCache.TryGetValue<IntegrationHostData>(integrationName, out var integrationHostData))
            {
                this.Logger.LogWarning("Failed to stop {integrationName}. No integration with that name is currently running.", integrationName);
                return;
            }

            if (!this.IsHostRunning(integrationHostData))
            {
                this.Logger.LogInformation("Attempted to stop {integrationName} but it is already stopped.", integrationName);
                return;
            }

            // Try to gracefully dispose of the Host.
            await integrationHostData.DisposeAsync();

            // Override the current Host with a stopped host data. 
            // This will cause OnCacheEntryEvicted to be called and make sure everything is cleaned up.
            this.IntegrationHostCache.Set(integrationName, new IntegrationHostData(Host: null, HostStatus.Stopped));
            this.Logger.LogInformation("{integrationName} has been stopped.", integrationName);
        }

        /// <summary>
        /// Disposes of the IMemoryCache used to track the Hosts
        /// This should cause it to evict all entries, causing them to be cleaned up.
        /// </summary>
        public void Dispose()
            => this.IntegrationHostCache.Dispose();

        private async Task<IntegrationHostData> CreateAndStartIntegrationHost(ICacheEntry cacheEntry, IntegrationRegistration integrationRegistration, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new IntegrationHostData(null, HostStatus.Errored) { Message = "Startup cancelled." };
            }

            // Pull a fresh copy of the IntegrationConfiguration to do some validation
            var integrationConfiguration = integrationRegistration.IntegrationConfiguration;
            var integrationName = integrationConfiguration.Name;
            var entryAssembly = integrationConfiguration.EntryAssembly;

            if (integrationName is null)
            {
                return new IntegrationHostData(null, HostStatus.Errored) { Message = "Integration Name not set properly in configuration." };
            }

            if (entryAssembly is null)
            {
                return new IntegrationHostData(null, HostStatus.Errored) { Message = "Entry assembly not set properly in configuration." };
            }

            if (!integrationConfiguration.IsEnabled)
            {
                this.Logger.LogInformation("Did not start {integrationName}. The Integration is currently disabled.", integrationName);
                return new IntegrationHostData(null, HostStatus.Disabled);
            }

            var entryAssemblyPath = Path.Combine(integrationRegistration.Directory, entryAssembly);
            if (!File.Exists(entryAssemblyPath))
            {
                this.Logger.LogError("Failed to Start {integrationName} because the entry assembly {entryAssemblyPath} could not be found", integrationName, entryAssemblyPath);
                return new IntegrationHostData(null, HostStatus.Errored) { Message = $"Failed to Start integration {integrationName} because the entry assembly {entryAssemblyPath} could not be found" };
            }

            // Use a custom Assembly load context to allow each Host to use its own dependencies.
            // If a custom dependency is not defined in the same folder, it will fallback to using any Assemblies loaded in the default context.
            // The only requirement is that the Mifs.dll cannot be different because that would result in different instances of IInitializationInstance's
            // making FindAndCreateIntegrationInitializationInstance never find a type to instantiate.
            var assemblyLoadContext = new IntegrationAssemblyLoadContext(integrationRegistration.Directory);
            var bootstrapInstance = this.FindAndCreateIntegrationBootstrapInstance(entryAssemblyPath, AssemblyLoadContext.Default);

            if (bootstrapInstance is null)
            {
                return new IntegrationHostData(null, HostStatus.Errored) { Message = "" };
            }

            var host = await this.IntegrationHostFactory.CreateAndStart(bootstrapInstance, integrationRegistration.Configuration, cancellationToken);
            if (host is null)
            {
                return new IntegrationHostData(host, HostStatus.Errored) { Message = $"Host failed to create" };
            }

            // Make sure that when the host is shut down the assembly load context gets unloaded.
            var applicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(() => assemblyLoadContext.Unload());

            cacheEntry.RegisterPostEvictionCallback(this.OnCacheEntryEvicted);

            this.Logger.LogInformation("{integrationName} Started.", integrationName);
            return new IntegrationHostData(host, HostStatus.Started);
        }

        private void OnCacheEntryEvicted(object key, object value, EvictionReason reason, object state)
        {
            // Make sure that if the value is disposable when it gets evicted that it gets disposed.
            // It should be disposed before hand, but this is a last check just to be safe.
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // If the host is not running for this integration now after the eviction
            // We want to clean up the server route.
            var integrationName = (string)key;
            if (!this.IsHostRunning(integrationName))
            {
                this.ProxyRouteCollection.RemoveServerRoute(integrationName);
            }
        }

        private IHostBootstrap? FindAndCreateIntegrationBootstrapInstance(string entryAssemblyPath, AssemblyLoadContext integrationAssemblyLoadContext)
        {
            // Use a file stream to load the assembly so that the dll's can be modified after it's been read
            using var fs = new FileStream(entryAssemblyPath, FileMode.Open, FileAccess.Read);
            var entryAssembly = integrationAssemblyLoadContext.LoadFromStream(fs);
            var bootstrapType = entryAssembly.ExportedTypes
                                             .Where(type => !type.IsInterface && !type.IsAbstract)
                                             .Where(type => typeof(IHostBootstrap).IsAssignableFrom(type))
                                             .Single();

            return Activator.CreateInstance(bootstrapType) as IHostBootstrap;
        }

        private void IntegrationRegistrar_OnIntegrationDeregistered(string integrationName)
            => this.IntegrationHostCache.Remove(integrationName); // Removing the cache entry causes OnCacheEntryEvicted to be called. 

        private async void IntegrationRegistrar_OnIntegrationFilesModified(string directory, string integrationName)
        {
            // TODO: If all the configuration files are gone we want to stop the host and leave it stopped.

            await this.StopHost(integrationName);
            await this.TryStartHost(integrationName);
        }

        /// <summary>
        /// Internal record type used to track the Hosts. 
        /// Provides a convinient way to dispose of the Host resources.
        /// </summary>
        private sealed record IntegrationHostData(IHost? Host, HostStatus Status) : IDisposable, IAsyncDisposable
        {
            public string? Message { get; init; }

            private bool HasDisposed { get; set; }

            public void Dispose()
            {
                if (this.HasDisposed) { return; }
                this.HasDisposed = true;

                if (this.Host is not null)
                {
                    this.Host.StopAsync().GetAwaiter().GetResult();
                    this.Host.Dispose();
                }
            }

            public async ValueTask DisposeAsync()
            {
                if (this.HasDisposed) { return; }
                this.HasDisposed = true;

                if (this.Host is not null)
                {
                    await this.Host.StopAsync();
                    this.Host.Dispose();
                }
            }
        }

    }
}
