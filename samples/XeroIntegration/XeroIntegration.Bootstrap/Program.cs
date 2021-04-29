using Microsoft.Extensions.Hosting;
using Mifs.Bootstrap;
using System.Threading.Tasks;

namespace XeroIntegration.Bootstrap
{
    /// <summary>
    /// In the production situation, there is a windows service that looks recursively through a folder structure for integrations to load.
    /// That would make for annoying debugging, so instead this simple console application acts as the entry point for the integration during debug.
    /// By calling Bootstrapper.CreateMifsHost() we are making the situation as close to production as possible.
    /// </summary>
    internal static class Program
    {
        private static Task Main(string[] args)
        {
            // Creates the Mifs host and runs it.
            // RunConsoleAsync returns a task that doesn't complete until the control is closed.
            return Bootstrapper.CreateMifsHost(args)
                               .RunConsoleAsync();
        }
    }
}
