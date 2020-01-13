using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class ApplicantInterviewNotificationBefore : IApplicantInterviewNotificationBefore
    {
        private readonly IApplicantInterviewNotificationBeforeService service;

        public ApplicantInterviewNotificationBefore(IApplicantInterviewNotificationBeforeService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
