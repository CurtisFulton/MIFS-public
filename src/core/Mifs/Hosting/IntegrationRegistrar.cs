using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mifs.Extensions;
using System;
using System.IO;

namespace Mifs.Hosting
{
    public record IntegrationRegistration(string Directory, IConfiguration Configuration)
    {
        public IntegrationConfiguration IntegrationConfiguration => this.Configuration.Get<IntegrationConfiguration?>("Integration")
                                                                                ?? throw new NullReferenceException($"Integration Registration for directory '{this.Directory}' is not valid because the configuration does not have a property 'Integration'.");

        public string Name => this.IntegrationConfiguration?.Name
                                            ?? throw new NullReferenceException($"Integration Registration for directory '{this.Directory}' is not valid because the configuration does not have a configuration value for 'Integration:Name'.");
    }

    /// <summary>
    /// Registrar for all integrations.
    /// Stores important information about each Integration
    /// and manages the SystemFileWatcher's.
    /// </summary>
    public class IntegrationRegistrar : IDisposable
    {
        public delegate void IntegrationRegisteredDelegate(string integrationName);
        public delegate void IntegrationDeregisteredDelegate(string integrationName);

        public IntegrationRegistrar(IntegrationFileWatcher integrationFileWatcher,
                                    ILogger<IntegrationRegistrar> logger)
        {
            this.IntegrationFileWatcher = integrationFileWatcher;
            this.Logger = logger;
        }

        public event IntegrationRegisteredDelegate? OnIntegrationRegistered;
        public event IntegrationDeregisteredDelegate? OnIntegrationDeregistered;
        public event IntegrationFilesModifiedDelegate? OnIntegrationFilesModified;

        private IMemoryCache IntegrationCache { get; } = new MemoryCache(new MemoryCacheOptions());

        private IntegrationFileWatcher IntegrationFileWatcher { get; }
        private ILogger<IntegrationRegistrar> Logger { get; }

        public void Register(IntegrationRegistration integrationRegistration)
        {
            var integrationName = integrationRegistration.Name;
            if (this.TryGetRegistration(integrationName, out var existingRegistration))
            {
                if (integrationRegistration.Directory != existingRegistration.Directory)
                {
                    this.Logger.LogWarning("Trying to register {integrationName} from directory {newDirectory} but it is already registered for directory {existingDirectory}. If you intended to change the directory deregister the old one first.",
                                                                integrationName, integrationRegistration.Directory, existingRegistration.Directory);
                }
                else
                {
                    this.Logger.LogInformation("Attempted to register {integrationName} but it is already registered.", integrationName);
                }

                return;
            }

            this.Logger.LogInformation("Registering {integrationName} to directory {directory}.", integrationName, integrationRegistration.Directory);
            this.IntegrationCache.GetOrCreate(integrationName,
                                              (entry) => this.AddIntegrationRegistration(entry, integrationRegistration));
        } 

        public void Deregister(string integrationName)
            => this.IntegrationCache.Remove(integrationName);

        public bool TryGetRegistration(string integrationName, out IntegrationRegistration integrationRegistration)
            => this.IntegrationCache.TryGetValue(integrationName, out integrationRegistration);

        public bool IsIntegrationRegistered(string integrationName)
            => this.TryGetRegistration(integrationName, out _);

        public IntegrationRegistration GetRegistration(string integrationName)
            => this.IntegrationCache.Get<IntegrationRegistration>(integrationName);

        /// <summary>
        /// Disposing of the underlying cache which will cause all of the entries to be evicted and cleaned up.
        /// </summary>
        public void Dispose()
            => this.IntegrationCache.Dispose();

        private IntegrationRegistration AddIntegrationRegistration(ICacheEntry cacheEntry, IntegrationRegistration integrationRegistration)
        {
            this.IntegrationFileWatcher.AddFileWatcher(integrationRegistration.Name, integrationRegistration.Directory, this.IntegrationFilesModified);

            cacheEntry.RegisterPostEvictionCallback(this.OnCacheEntryEvicted);

            this.OnIntegrationRegistered?.Invoke(integrationRegistration.Name);
            return integrationRegistration;
        }

        private void IntegrationFilesModified(string directory, string integrationName)
        {
            if (!this.IntegrationCache.TryGetValue<IntegrationRegistration>(integrationName, out var integrationRegistration))
            {
                this.Logger.LogWarning("File changes detected in directory {integrationDirectory} but there is no integration registered with the name {integrationName}", directory, integrationName);
                return;
            }
            
            // Remove this integration registration if the directory no longer exists
            if (!Directory.Exists(directory))
            {
                this.Logger.LogInformation("Removing {integrationName} registration because the directory {integrationDirectory} no longer exists.", integrationName, directory);
                this.IntegrationCache.Remove(integrationName);
                return;
            }

            this.OnIntegrationFilesModified?.Invoke(directory, integrationName);
        }

        private void OnCacheEntryEvicted(object key, object value, EvictionReason reason, object state)
        {
            var integrationName = (string)key;
            this.Logger.LogInformation("Removing {integrationName} registration.", integrationName);

            this.IntegrationFileWatcher.RemoveFileWatcher(integrationName);
            this.OnIntegrationDeregistered?.Invoke(integrationName);

            this.Logger.LogInformation("{integrationName} registration has been removed.", integrationName);
        }
    }
}
