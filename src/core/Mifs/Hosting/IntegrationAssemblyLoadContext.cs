using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Mifs.Hosting
{
    /// <summary>
    /// Custom assembly load context.
    /// It will search in the same directory as the initial assembly for the dependency
    /// and fallback to the default AssemblyLoadContext if it doesn't exist.
    /// The loading of assemblies is done through a FileStream to allow the dll to be released once loaded.
    /// </summary>
    internal class IntegrationAssemblyLoadContext : AssemblyLoadContext
    {
        public IntegrationAssemblyLoadContext(string entryAssemblyPath) : base(isCollectible: true)
        {
            this.AssemblyDependencyResolver = new AssemblyDependencyResolver(entryAssemblyPath);
        }

        private AssemblyDependencyResolver AssemblyDependencyResolver { get; }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var assemblyPath = this.AssemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath is not null)
            {
                // Use a file stream to load the assembly in.
                // This seems to allow the dll to be modified after it has been loaded
                using var fs = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read);
                return this.LoadFromStream(fs);
            }

            return null;
        }
    }
}
