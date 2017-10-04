using Hangfire;
using System;

namespace Sofco.WebJob.Services
{
    public class JobService
    {
        public static void Init()
        {
            RecurringJob.AddOrUpdate(
                () => Console.WriteLine("Recurring!"),
                Cron.Daily);
        }
    }
}
