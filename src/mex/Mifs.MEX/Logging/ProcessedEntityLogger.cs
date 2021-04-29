using Microsoft.Extensions.Configuration;
using Mifs.MEX.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.MEX.Logging
{
    internal class ProcessedEntityLogger<TEntity> : IProcessedEntityLogger<TEntity>
    {
        public ProcessedEntityLogger(IMEXDbContext dbContext,
                                     IConfiguration configuration)
        {
            this.DbContext = dbContext;
            this.IntegrationName = configuration.GetValue<string>("Integration:Name");
        }

        private IMEXDbContext DbContext { get; }
        private string IntegrationName { get; }

        public async Task<int> LogFailure(int entityId, string failureMessage, CancellationToken cancellationToken = default)
        {
            var entityType = typeof(TEntity).Name;
            var logRecord = new MifsProcessedEntityLog()
            {
                EntityId = entityId,
                EntityName = entityType,
                IntegrationName = this.IntegrationName,
                ErrorMessage = failureMessage
            };

            this.DbContext.MifsProcessedEntityLogs.Add(logRecord);
            await this.DbContext.SaveChangesAsync(cancellationToken);

            return logRecord.MifsProcessedEntityLogId;
        }

        public async Task<int> LogSuccess(int entityId, CancellationToken cancellationToken = default)
        {
            var entityType = typeof(TEntity).Name;
            var logRecord = new MifsProcessedEntityLog()
            {
                EntityId = entityId,
                EntityName = entityType,
                IntegrationName = this.IntegrationName,
                ProcessedDateTime = DateTime.Now
            };

            this.DbContext.MifsProcessedEntityLogs.Add(logRecord);
            await this.DbContext.SaveChangesAsync(cancellationToken);

            return logRecord.MifsProcessedEntityLogId;
        }
    }
}
