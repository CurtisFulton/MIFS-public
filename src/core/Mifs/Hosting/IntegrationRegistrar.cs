using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mifs.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

            this.OnIntegrationDeregistered += IntegrationDeregisteredCallback;
        }

        public event IntegrationRegisteredDelegate? OnIntegrationRegistered;
        public event IntegrationDeregisteredDelegate? OnIntegrationDeregistered;
        public event IntegrationFilesModifiedDelegate? OnIntegrationFilesModified;

        private ConcurrentDictionary<string, IntegrationRegistration> AllIntegrations { get; } = new ConcurrentDictionary<string, IntegrationRegistration>();

        private IntegrationFileWatcher IntegrationFileWatcher { get; }
        private ILogger<IntegrationRegistrar> Logger { get; }

        public void Register(IntegrationRegistration integrationRegistration)
        {
            var integrationName = integrationRegistration.Name;
            if (this.TryGetRegistration(integrationName, out var existingRegistration))
            {
                if (existingRegistration is not null && integrationRegistration.Directory != existingRegistration.Directory)
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
            this.AllIntegrations.GetOrAdd(integrationName,
                                          _ => this.AddIntegrationRegistration(integrationRegistration));
        }

        public void Deregister(string integrationName)
        {
            if (this.AllIntegrations.Remove(integrationName, out var _))
            {
                this.Logger.LogInformation("Removing {integrationName} registration.", integrationName);
                this.OnIntegrationDeregistered?.Invoke(integrationName);
                this.Logger.LogInformation("{integrationName} registration has been removed.", integrationName);
            }
        }
            
        public bool TryGetRegistration(string integrationName, out IntegrationRegistration? integrationRegistration)
            => this.AllIntegrations.TryGetValue(integrationName, out integrationRegistration);

        public bool IsIntegrationRegistered(string integrationName)
            => this.AllIntegrations.ContainsKey(integrationName);

        public ICollection<IntegrationRegistration> GetIntegrations()
            => this.AllIntegrations.Values;

        /// <summary>
        /// Deregister all integrations and clean up handlers.
        /// </summary>
        public void Dispose()
        {
            foreach (var integrationName in this.AllIntegrations.Keys)
            {
                if (integrationName is null)
                {
                    continue;
                }

                this.Deregister(integrationName);
            }

            this.OnIntegrationRegistered = null;
            this.OnIntegrationDeregistered = null;
            this.OnIntegrationFilesModified = null;
        }

        private IntegrationRegistration AddIntegrationRegistration(IntegrationRegistration integrationRegistration)
        {
            this.IntegrationFileWatcher.AddFileWatcher(integrationRegistration.Name, integrationRegistration.Directory, this.IntegrationFilesModified);

            this.OnIntegrationRegistered?.Invoke(integrationRegistration.Name);
            return integrationRegistration;
        }

        private void IntegrationFilesModified(string directory, string integrationName)
        {
            if (!this.TryGetRegistration(integrationName, out var integrationRegistration))
            {
                this.Logger.LogWarning("File changes detected in directory {integrationDirectory} but there is no integration registered with the name {integrationName}", directory, integrationName);
                return;
            }
            
            // Remove this integration registration if the directory no longer exists
            if (!Directory.Exists(directory))
            {
                this.Logger.LogInformation("Removing {integrationName} registration because the directory {integrationDirectory} no longer exists.", integrationName, directory);
                this.Deregister(integrationName);
                return;
            }

            this.OnIntegrationFilesModified?.Invoke(directory, integrationName);
        }

        private void IntegrationDeregisteredCallback(string integrationName)
        {
            this.IntegrationFileWatcher.RemoveFileWatcher(integrationName);
        }
    }
}
