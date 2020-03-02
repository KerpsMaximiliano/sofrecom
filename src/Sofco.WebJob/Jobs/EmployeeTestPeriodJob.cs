using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeTestPeriodJob : IEmployeeTestPeriodJob
    {
        private readonly IEmployeeTestPeriodJobService service;

        public EmployeeTestPeriodJob(IEmployeeTestPeriodJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.Execute();
        }
    }
}
