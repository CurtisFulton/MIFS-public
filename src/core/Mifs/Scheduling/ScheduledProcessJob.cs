using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mifs.Scheduling
{
    [DisallowConcurrentExecution]
    internal class ScheduledProcessJob<TDataProvider, TEntityProcessor, TEntity> : IJob
            where TDataProvider : class, IScheduledDataProvider<TEntity>
            where TEntityProcessor : class, IProcessEntity<TEntity>
    {
        public ScheduledProcessJob(TDataProvider dataProvider, 
                                   TEntityProcessor entityProcessor)
        {
            this.DataProvider = dataProvider;
            this.EntityProcessor = entityProcessor;
        }

        public TDataProvider DataProvider { get; }
        public TEntityProcessor EntityProcessor { get; }

        public async Task Execute(IJobExecutionContext context)
        {
            var cancellationToken = context.CancellationToken;

            if (this.DataProvider is IInitialize dataProviderInitializer)
            {
                await dataProviderInitializer.Initialize(cancellationToken);
            }

            if (this.DataProvider != this.EntityProcessor && this.EntityProcessor is IInitialize entityProcessorInitializer)
            {
                await entityProcessorInitializer.Initialize(cancellationToken);
            }

            var data = this.DataProvider.GetData(cancellationToken);
            await foreach (var item in data.WithCancellation(cancellationToken))
            {
                await this.ProcessSingleItem(item, cancellationToken);
            }
        }

        private async Task ProcessSingleItem(TEntity item, CancellationToken cancellationToken)
        {
            try
            {
                await this.EntityProcessor.Process(item, cancellationToken);
            }
            catch (Exception ex)
            {
                var didHandle = await this.EntityProcessor.HandleError(item, ex, cancellationToken);
                if (!didHandle)
                {
                    throw;
                }
            }
        }
    }
}
