using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeSyncJob : IEmployeeSyncJob
    {
        private readonly IEmployeeSyncJobService jobService;

        public EmployeeSyncJob(IEmployeeSyncJobService jobService)
        {
            this.jobService = jobService;
        }

        public void Execute()
        {
            jobService.SyncNewEmployees();

            jobService.SyncEndEmployees();
        }
    }
}
