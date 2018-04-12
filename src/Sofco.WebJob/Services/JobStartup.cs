using System;
using Hangfire;
using Sofco.WebJob.Services.Interfaces;

namespace Sofco.WebJob.Services
{
    public class JobStartup
    {
        public static void Init()
        {
            BackgroundJob.Schedule<IJobService>(j => j.Execute(), DateTime.UtcNow.AddSeconds(1));
        }
    }
}
