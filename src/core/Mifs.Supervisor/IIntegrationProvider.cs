using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mifs.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mifs.Supervisor
{
    public interface IIntegrationProvider
    {
        IAsyncEnumerable<IntegrationRegistration> GetIntegrations();
    }

    /// <summary>
    /// Looks in the specified folder(s) in the configuration for any *appsettings.json files.
    /// Integrations will be considered valid for registration when they have both a name and EntryAssembly defined in the config.
    /// </summary>
    public class DefaultIntegrationProvider : IIntegrationProvider
    {
        public DefaultIntegrationProvider(IConfiguration configuration,
                                       ILogger<DefaultIntegrationProvider> logger)
        {
            this.Configuration = configuration;
            this.Logger = logger;
        }

        private IConfiguration Configuration { get; }
        private ILogger<DefaultIntegrationProvider> Logger { get; }

        public async IAsyncEnumerable<IntegrationRegistration> GetIntegrations()
        {
            var rootDirectory = Path.GetFullPath(this.Configuration.GetValue<string>("Mifs:IntegrationPath"));

            var integrationRegistrations = new List<IntegrationRegistration>();
            var configurationFilesByDirectory = Directory.EnumerateFiles(rootDirectory, "*appsettings.json", SearchOption.AllDirectories)
                                                         .GroupBy(file => Path.GetDirectoryName(file));

            foreach (var configurationFiles in configurationFilesByDirectory)
            {
                var directory = Path.GetFullPath(configurationFiles.Key!);
                var configuration = this.BuildConfigurationForIntegration(configurationFiles);
                var integrationConfiguration = configuration.GetSection("Integration")
                                                            .Get<IntegrationConfiguration>();

                if (!this.IsIntegrationConfigurationValid(integrationConfiguration))
                {
                    this.Logger.LogWarning("Failed to register Integration in directory '{directory}' because it was invalid. Check its configuration contains the Integration Name and Entry Assembly.", directory);
                    continue;
                }

                this.Logger.LogInformation("Found Integration {integrationName} in directory {directory}.", integrationConfiguration.Name, directory);
                var integrationRegistration = new IntegrationRegistration(directory, configuration);
                yield return integrationRegistration;

                await Task.Yield();
            }
        }

        private bool IsIntegrationConfigurationValid(IntegrationConfiguration integrationConfiguration)
        {
            if (integrationConfiguration.Name.IsNullOrWhiteSpace())
            {
                return false;
            }

            if (integrationConfiguration.EntryAssembly.IsNullOrWhiteSpace())
            {
                return false;
            }

            return true;
        }

        private IConfiguration BuildConfigurationForIntegration(IGrouping<string?, string?> configurationFiles)
        {
            // Using the configuration files in this directory, we want to build up a configuration object
            var configurationBuilder = new ConfigurationBuilder();
            foreach (var configurationFile in configurationFiles)
            {
                if (configurationFile is null)
                {
                    continue;
                }

                var configurationFileFullPath = Path.GetFullPath(configurationFile);
                configurationBuilder.AddJsonFile(configurationFileFullPath, optional: false, reloadOnChange: true);
            }

            // Build the configuration and get the IntegrationConfiguration object from it
            return configurationBuilder.Build();
        }
    }
}
