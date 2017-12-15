using System;
using System.Linq;
using Hangfire;
using Hangfire.Storage;

namespace Sofco.WebJob.Helpers
{
    public class JobHelper
    {
        public static void ClearScheduledJob(params Type[] jobTypes)
        {
            if (jobTypes == null) throw new ArgumentNullException(nameof(jobTypes));

            var monitor = JobStorage.Current.GetMonitoringApi();

            var scheduledCount = (int)monitor.ScheduledCount();

            foreach (var scheduledJobDto in monitor.ScheduledJobs(0, scheduledCount)
                .Where(scheduledJobDto => jobTypes.Any(_ => _ == scheduledJobDto.Value.Job.Type)))
            {
                BackgroundJob.Delete(scheduledJobDto.Key);
            }
        }

        public static void ClearAllScheduledJob()
        {
            var monitor = JobStorage.Current.GetMonitoringApi();

            var scheduledCount = (int)monitor.ScheduledCount();

            foreach (var scheduledJobDto in monitor.ScheduledJobs(0, scheduledCount))
            {
                BackgroundJob.Delete(scheduledJobDto.Key);
            }
        }

        public static void ClearAllRecurringJob()
        {
            var recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();
            foreach (var job in recurringJobs)
            {
                RecurringJob.RemoveIfExists(job.Id);
            }
        }

        public static bool IsScheduledJob(Type jobType)
        {
            var monitor = JobStorage.Current.GetMonitoringApi();

            var scheduledCount = (int)monitor.ScheduledCount();

            return monitor.ScheduledJobs(0, scheduledCount)
                    .Any(scheduledJobDto => jobType == scheduledJobDto.Value.Job.Type);
        }
    }
}