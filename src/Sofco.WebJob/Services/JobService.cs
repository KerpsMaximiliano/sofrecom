using Hangfire;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Services
{
    public class JobService
    {
        private const string SolfacJobName = "SolfacDailyJob";

        public static void Init()
        {
            RecurringJob.AddOrUpdate<ISolfacJob>(SolfacJobName, j => j.Execute(), Cron.Daily);
        }
    }
}
