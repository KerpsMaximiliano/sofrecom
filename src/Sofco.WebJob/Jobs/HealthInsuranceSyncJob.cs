using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class HealthInsuranceSyncJob : IHealthInsuranceSyncJob
    {
        private readonly IHealthInsuranceJobService jobService;

        public HealthInsuranceSyncJob(IHealthInsuranceJobService jobService)
        {
            this.jobService = jobService;
        }

        public void Execute()
        {
            jobService.SyncHealthInsurance();

            jobService.SyncPrepaidHealth();
        }
    }
}
