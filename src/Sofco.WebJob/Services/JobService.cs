using Hangfire;
using Sofco.WebJob.Jobs.Interfaces;
using System;

namespace Sofco.WebJob.Services
{
    public class JobService
    {
        private const string SolfacJobName = "SolfacDailyJob";

        public static void Init(string timeZoneName)
        {
            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

            RecurringJob.AddOrUpdate<ISolfacJob>(SolfacJobName, j => j.Execute(), Cron.Daily(8), localTimeZone);
        }
    }
}
