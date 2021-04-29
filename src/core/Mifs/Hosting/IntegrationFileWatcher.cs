using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reactive.Linq;

namespace Mifs.Hosting
{
    public delegate void IntegrationFilesModifiedDelegate(string directory, string integrationName);

    /// <summary>
    /// File watcher helper for Integrations.
    /// Removes all specific data from Create/Delete/Rename/Modify events and treats them all equally.
    /// Throttling is applied so that for all events combined it'll only fire at a rate of 1 per second.
    /// </summary>
    public class IntegrationFileWatcher : IDisposable
    {
        private record FileWatcherData(FileSystemWatcher FileSystemWatcher, IDisposable FileWatcherObserableSubscription);

        public IntegrationFileWatcher(IHostApplicationLifetime applicationLifetime,
                                      ILogger<IntegrationFileWatcher> logger)
        {
            this.ApplicationLifetime = applicationLifetime;
            this.Logger = logger;
        }

        private IHostApplicationLifetime ApplicationLifetime { get; }
        private ILogger<IntegrationFileWatcher> Logger { get; }

        private IMemoryCache FileWatcherCache { get; } = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Adds a File watcher under the specified integration name and directory.
        /// Allows a callback to be registered for when any file Create/Delete/Rename/Modify occurs.
        /// </summary>
        /// <param name="integrationName">Name of the registration. Used later to remove the file watcher.</param>
        /// <param name="directory">Directory to watch.</param>
        /// <param name="actionCallback">Callback for when any Create/Delete/Rename/Modify event occurs (All data about the event is lost). Wont occur more than once every second</param>
        public void AddFileWatcher(string integrationName, string directory, IntegrationFilesModifiedDelegate actionCallback)
        {
            // Create 2 entries.
            // First one is to map an integration name to a directory. This makes it so we can deregister by integration name
            // Second one is to store the actual file watcher data that we need to dispose of later.
            this.FileWatcherCache.Set(integrationName, directory);
            this.FileWatcherCache.GetOrCreate(directory, (entry) => this.CreateFileWatcherForIntegration(entry, integrationName, directory, actionCallback));
        }
            
        /// <summary>
        /// Removes the file watcher and all event handlers for the specified integration
        /// </summary>
        /// <param name="integrationName">Name of the integration to remove the file watcher for</param>
        public void RemoveFileWatcher(string integrationName)
        {
            if (!this.FileWatcherCache.TryGetValue<string>(integrationName, out var directory))
            {
                this.Logger.LogWarning("Tried to remove file watcher for {integrationName} but no integration with that name was registered.", integrationName);
                return;
            }

            this.FileWatcherCache.Remove(integrationName);
            this.FileWatcherCache.Remove(directory);
        }

        private FileWatcherData CreateFileWatcherForIntegration(ICacheEntry cacheEntry, string integrationName, string directory, IntegrationFilesModifiedDelegate callback)
        {
            var fileWatcher = new FileSystemWatcher(directory)
            {
                NotifyFilter = NotifyFilters.Attributes | NotifyFilters.FileName | NotifyFilters.LastWrite,
                IncludeSubdirectories = false,
                EnableRaisingEvents = true
            };

            // Add observables for each of the event types.
            // Each of them just returns a boolean because we need a shared return type from the observable. 
            // We only care that *something* changed, we don't care what actually changed.
            var changedFileObserable = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => fileWatcher.Changed += h, h => fileWatcher.Changed -= h)
                                                 .Select(x => true);

            var createdFileObserable = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => fileWatcher.Created += h, h => fileWatcher.Created -= h)
                                                 .Select(x => true);

            var deletedFileObserable = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(h => fileWatcher.Deleted += h, h => fileWatcher.Deleted -= h)
                                                 .Select(x => true);

            var renamedFileObserable = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(h => fileWatcher.Renamed += h, h => fileWatcher.Renamed -= h)
                                                 .Select(x => true);

            // Merge all the different file watcher events into 1 observable
            // This allows us to throttle all the events coming through so we don't get more than 1 per second.
            var fileWatcherSubscription = Observable.Merge(changedFileObserable, createdFileObserable, deletedFileObserable, renamedFileObserable)
                                                    .Throttle(TimeSpan.FromSeconds(1))
                                                    .Subscribe(_ => callback?.Invoke(directory, integrationName));

            cacheEntry.RegisterPostEvictionCallback(this.OnCacheEntryEvicted);
            this.ApplicationLifetime.ApplicationStopping.Register(() => this.RemoveFileWatcher(integrationName));

            return new FileWatcherData(fileWatcher, fileWatcherSubscription);
        }

        private void OnCacheEntryEvicted(object key, object value, EvictionReason reason, object state)
        {
            if (value is not FileWatcherData fileWatcherData)
            {
                this.Logger.LogError("Tried to remove File watcher for integration {integrationName} but it's data wasn't stored in the cache as FileWatcherData.", key.ToString());
                return;
            }

            this.Logger.LogInformation("Removing File watcher for integration {integrationName}", key.ToString());

            fileWatcherData.FileSystemWatcher.Dispose();
            fileWatcherData.FileWatcherObserableSubscription.Dispose();
        }

        public void Dispose()
        {
            this.FileWatcherCache.Dispose();
        }
    }
}
