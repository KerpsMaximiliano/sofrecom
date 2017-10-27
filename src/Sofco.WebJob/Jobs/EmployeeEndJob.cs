using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeEndJob : IEmployeeEndJob
    {
        private readonly IEmployeeEndJobService service;

        public EmployeeEndJob(IEmployeeEndJobService service)
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
