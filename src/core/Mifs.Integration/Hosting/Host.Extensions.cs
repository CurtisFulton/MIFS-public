using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Mifs.Hosting
{
    public static class Host_Extensions
    {
        /// <summary>
        /// Migrates the specified DbContext asynchronously.
        /// Requires migrations for the DbContext to exist.
        /// </summary>
        /// <typeparam name="TContext">Type of the DbContext to migrate.</typeparam>
        /// <param name="host">Host that has the DbContext configured</param>
        /// <returns></returns>
        public static async Task MigrateDbContext<TContext>(this IHost host) where TContext : DbContext
        {
            using var serviceScope = host.Services.CreateScope();
            var scopedServiceProvider = serviceScope.ServiceProvider;

            var dbContext = scopedServiceProvider.GetRequiredService<TContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
