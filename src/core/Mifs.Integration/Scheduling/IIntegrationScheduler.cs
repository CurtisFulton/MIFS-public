using Microsoft.Extensions.Configuration;
using Mifs.Extensions;
using Quartz;
using System.Threading.Tasks;

namespace Mifs.Scheduling
{
    /// <summary>
    /// Interface used to schedule jobs in Mifs.
    /// Currently only supports using Cron Schedules.
    /// </summary>
    public interface IIntegrationScheduler
    {
        /// <summary>
        /// Schedules a job to be run when any of the cron schedules trigger.
        /// </summary>
        /// <typeparam name="TJob">Type of the Job to execute</typeparam>
        /// <param name="cronSchedules">Array of all the cron schedules to trigger this job</param>
        /// <param name="executeOnStartup">If true, this job will execute when the integration started, even if the cron schedules are not true</param>
        /// <returns>Task representing when the job has been setup</returns>
        Task ScheduleJob<TJob>(string[] cronSchedules, bool executeOnStartup = false) where TJob : IScheduledJob;

        /// <summary>
        /// Schedules a job to be run when any of the cron schedules trigger.
        /// This provides a more structured way for the job to be executed.
        /// When a job is scheduled this way, error handling and processing is handled for you.
        /// This is the recommended way to run scheduled jobs.
        /// </summary>
        /// <typeparam name="TJob">Type of the Job to execute</typeparam>
        /// <typeparam name="TSource">Type of the entity that the Job Processes.</typeparam>
        /// <param name="cronSchedules">Array of all the cron schedules to trigger this job</param>
        /// <param name="executeOnStartup">If true, this job will execute when the integration started, even if the cron schedules are not true</param>
        /// <returns>Task representing when the job has been setup</returns>
        Task ScheduleJob<TJob, TSource>(string[] cronSchedules, bool executeOnStartup = false)
            where TJob : class, IScheduledDataProvider<TSource>, IProcessEntity<TSource>;
    }

    /// <summary>
    /// Default implementation of the IntegrationScheduler.
    /// Currently uses Quartz as the scheduling engine.
    /// </summary>
    internal class DefaultIntegrationScheduler : IIntegrationScheduler
    {
        public DefaultIntegrationScheduler(ISchedulerFactory schedulerFactory,
                                           IConfiguration configuration)
        {
            this.SchedulerFactory = schedulerFactory;
            this.Configuration = configuration;
        }

        private ISchedulerFactory SchedulerFactory { get; }
        private IConfiguration Configuration { get; }

        public Task ScheduleJob<TJob>(string[] defaultCronSchedules, bool executeOnStartup = false)
            where TJob : IScheduledJob
            => this.CreateAndScheduleJob<BasicScheduledJobWrapper<TJob>>(typeof(TJob).Name, defaultCronSchedules, executeNow: executeOnStartup);

        public Task ScheduleJob<TJob, TSource>(string[] defaultCronSchedules, bool executeOnStartup = false)
            where TJob : class, IScheduledDataProvider<TSource>, IProcessEntity<TSource>
            => this.CreateAndScheduleJob<ScheduledProcessJob<TJob, TJob, TSource>>(typeof(TJob).Name, defaultCronSchedules, executeNow: executeOnStartup);

        private async Task CreateAndScheduleJob<TJob>(string jobName, string[] defaultCronSchedules, bool executeNow)
            where TJob : IJob
        {
            // Currently we aren't storing the schedules durably because we don't need that functionality
            // And having it stored in memory means we don't have to worry about schedule cleanup when jobs schedule is changed.
            var scheduler = await this.SchedulerFactory.GetScheduler();
            var jobDetail = JobBuilder.Create<TJob>()
                                      .WithIdentity(jobName)
                                      .StoreDurably()
                                      .Build();

            await scheduler.AddJob(jobDetail, true);

            // Check if there is any cron schedules stored in the configuration for this job.
            // If there isn't, we fallback to using the hard coded default values.
            var cronSchedules = this.Configuration.Get<string[]>($"{jobName}:CronSchedules")
                                        ?? defaultCronSchedules;

            // Because there can be multiple cron schedules provided, we need to create a separate trigger for each one.
            foreach (var cronSchedule in cronSchedules)
            {
                var trigger = TriggerBuilder.Create()
                                            .WithIdentity($"{jobName}.Trigger({cronSchedule})")
                                            .WithDescription($"Trigger for Job '{jobName}' with Cron Schedule: '{cronSchedule}'")
                                            .WithCronSchedule(cronSchedule)
                                            .ForJob(jobDetail)
                                            .Build();

                await scheduler.ScheduleJob(trigger);
            }

            // If they specified they want it to execute now, we want to manually trigger the job.
            if (executeNow)
            {
                await scheduler.TriggerJob(jobDetail.Key);
            }
        }
    }
}
