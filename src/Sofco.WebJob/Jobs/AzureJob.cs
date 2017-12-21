using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class AzureJob : IAzureJob
    {
        private readonly IAzureAddUsersJobService jobService;

        public AzureJob(IAzureAddUsersJobService jobService)
        {
            this.jobService = jobService;
        }

        public void Execute()
        {
            jobService.UpdateUsersFromAzureAd();
        }
    }
}
