using Microsoft.Extensions.Logging;
using Mifs.Scheduling;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace XeroIntegration.Export
{
    public class ExampleFakeExport : IScheduledDataProvider<MyCustomExportEntity>, IProcessEntity<MyCustomExportEntity>
    {
        public ExampleFakeExport(ILogger<ExampleFakeExport> logger)
        {
            this.Logger = logger;
        }

        private ILogger<ExampleFakeExport> Logger { get; }

        public async IAsyncEnumerable<MyCustomExportEntity> GetData([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            // Normally data would be pulled from a database/FTP
            for (int i = 0; i < 2; i++)
            {
                yield return new MyCustomExportEntity(i);
                await Task.Yield();
            }
        }

        public Task<bool> HandleError(MyCustomExportEntity entity, Exception exception, CancellationToken cancellationToken)
        {
            this.Logger.LogError("Failed to process entity");
            return Task.FromResult(true);
        }

        public Task Process(MyCustomExportEntity entity, CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Processing Example export entity Entity {0}", entity.Id);

            return Task.CompletedTask;
        }
    }

    public class MyCustomExportEntity
    {
        public MyCustomExportEntity(int Id)
        {
            this.Id = Id;
        }

        public int Id { get; }
    }
}
