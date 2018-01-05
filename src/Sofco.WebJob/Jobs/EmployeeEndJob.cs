using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeEndJob : IEmployeeEndJob
    {
        private readonly IEmployeeEndNotificationJobService service;

        public EmployeeEndJob(IEmployeeEndNotificationJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            SendNotification();
        }

        public void SendNotification()
        {
            service.SendNotification();
        }
    }
}
