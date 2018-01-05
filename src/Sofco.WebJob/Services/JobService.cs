using System;
using Hangfire;
using Sofco.WebJob.Helpers;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Services
{
    public class JobService
    {
        private const string SolfacJobName = "HitosSinSolfacDailyJob";

        private const string TigerEmployeeSyncJobName = "TigerEmployeeSyncJob";

        private const string AzureJobName = "UpdateAzureUsers";

        public static void Init(string timeZoneName)
        {
            ClearJobs();

            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

            RecurringJob.AddOrUpdate<ISolfacJob>(SolfacJobName, j => j.Execute(), Cron.Daily(8), localTimeZone);

            RecurringJob.AddOrUpdate<IAzureJob>(AzureJobName, j => j.Execute(), Cron.Weekly(DayOfWeek.Monday), localTimeZone);

            RecurringJob.AddOrUpdate<IEmployeeSyncJob>(TigerEmployeeSyncJobName, j => j.Execute(), Cron.Daily(7), localTimeZone);
        }

        private static void ClearJobs()
        {
            JobHelper.ClearAllRecurringJob();
        }
    }
}
