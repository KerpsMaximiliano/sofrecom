using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class ApplicantInterviewNotificationAfter : IApplicantInterviewNotificationAfter
    {
        private readonly IApplicantInterviewNotificationAfterService service;

        public ApplicantInterviewNotificationAfter(IApplicantInterviewNotificationAfterService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
