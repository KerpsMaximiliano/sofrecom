using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class LicenseDaysUpdateJob : ILicenseDaysUpdateJob
    {
        private readonly ILicenseDaysUpdateJobService licenseDaysUpdateJobService;

        public LicenseDaysUpdateJob(ILicenseDaysUpdateJobService licenseDaysUpdateJobService)
        {
            this.licenseDaysUpdateJobService = licenseDaysUpdateJobService;
        }

        public void Execute()
        {
            licenseDaysUpdateJobService.Run();
        }
    }
}
