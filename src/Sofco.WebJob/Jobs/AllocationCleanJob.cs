using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class AllocationCleanJob : IAllocationCleanJob
    {
        private readonly IAllocationCleanJobService service;

        public AllocationCleanJob(IAllocationCleanJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Clean();
        }
    }
}
