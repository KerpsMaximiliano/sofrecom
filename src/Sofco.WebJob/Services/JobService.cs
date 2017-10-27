using System;
using Hangfire;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Services
{
    public class JobService
    {
        private const string SolfacJobName = "HitosSinSolfacDailyJob";

        private const string EmployeeSyncJobName = "EmployeeSyncJob";

        private const string EmployeeEndJobName = "EmployeeEndNotificationJob";

        public static void Init(string timeZoneName)
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

            RecurringJob.AddOrUpdate<ISolfacJob>(SolfacJobName, j => j.Execute(), Cron.Daily(8), localTimeZone);

            RecurringJob.AddOrUpdate<IEmployeeSyncJob>(EmployeeSyncJobName, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IEmployeeEndJob>(EmployeeEndJobName, j => j.SendNotification(), Cron.Daily(7, 30), localTimeZone);
        }
    }
}
