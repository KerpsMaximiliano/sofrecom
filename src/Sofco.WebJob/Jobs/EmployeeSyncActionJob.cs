using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeSyncActionJob : IEmployeeSyncActionJob
    {
        private readonly IEmployeeSyncActionJobService jobService;

        public EmployeeSyncActionJob(IEmployeeSyncActionJobService jobService)
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
