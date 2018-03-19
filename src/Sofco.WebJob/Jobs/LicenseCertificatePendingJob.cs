using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class LicenseCertificatePendingJob : ILicenseCertificatePendingJob
    {
        public const string JobName = "LicenseCertificatePending";

        private readonly ILicenseCertificatePendingJobService service;

        public LicenseCertificatePendingJob(ILicenseCertificatePendingJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.SendNotifications();
        }
    }
}
