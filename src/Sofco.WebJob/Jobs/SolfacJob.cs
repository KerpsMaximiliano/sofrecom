using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class SolfacJob : ISolfacJob
    {
        private readonly ISolfacJobService jobService;

        public SolfacJob(ISolfacJobService jobService)
        {
            this.jobService = jobService;
        }

        public void Execute()
        {
            jobService.SendHitosNotfications();
        }
    }
}
