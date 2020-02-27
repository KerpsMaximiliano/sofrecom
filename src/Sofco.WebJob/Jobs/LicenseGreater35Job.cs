using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class LicenseGreater35Job : ILicenseGreater35Job
    {
        private readonly ILicenseGreater35JobService service;

        public LicenseGreater35Job(ILicenseGreater35JobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
