using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Mifs.MEX.Domain;
using System;
using System.Threading.Tasks;

namespace Mifs.MEX.Services
{
    public interface ISystemOptionService
    {
        Task<SystemOption> GetSystemOption();
    }

    internal class SystemOptionService : ISystemOptionService
    {
        private static string SystemOptionCacheKey { get; } = $"{nameof(SystemOptionService)}_{nameof(SystemOption)}";

        public SystemOptionService(IFactory<IMEXDbContext> mexDbContextFactory,
                                   IMemoryCache memoryCache)
        {
            this.MexDbContextFactory = mexDbContextFactory;
            this.MemoryCache = memoryCache;
        }

        private IFactory<IMEXDbContext> MexDbContextFactory { get; }
        private IMemoryCache MemoryCache { get; }

        public Task<SystemOption> GetSystemOption()
            => this.MemoryCache.GetOrCreateAsync(SystemOptionService.SystemOptionCacheKey, this.GetSystemOptionFromDatabase);

        private async Task<SystemOption> GetSystemOptionFromDatabase(ICacheEntry cacheEntry)
        {
            // Refresh the system options every 10 minutes
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            using var mexDbContext = this.MexDbContextFactory.Create();
            var systemOption = await mexDbContext.SystemOptions.SingleAsync();
            return systemOption;
        }
    }
}
