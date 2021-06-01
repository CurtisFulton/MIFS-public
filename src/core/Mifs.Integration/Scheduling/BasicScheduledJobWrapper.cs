using Quartz;
using System.Threading.Tasks;

namespace Mifs.Scheduling
{
    /// <summary>
    /// Internal Wrapper class around quartz so the actual interface code doesn't have a dependency on Quartz.
    /// </summary>
    /// <typeparam name="TJob">Scheduled Integration Type</typeparam>
    [DisallowConcurrentExecution]
    internal class BasicScheduledJobWrapper<TJob> 
        : IJob where TJob : IScheduledJob
    {
        public BasicScheduledJobWrapper(TJob interfaceInstance)
        {
            this.InterfaceInstance = interfaceInstance;
        }

        private TJob InterfaceInstance { get; }

        public async Task Execute(IJobExecutionContext context)
            => await this.InterfaceInstance.Execute(context.CancellationToken);
    }
}
