using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeResetExamDaysJob : IEmployeeResetExamDaysJob
    {
        private readonly IEmployeeResetExamDaysJobService service;

        public EmployeeResetExamDaysJob(IEmployeeResetExamDaysJobService service)
        {
            this.service = service;
        }

        public void Execute()
        {
            service.ResetExamDays();
        }
    }
}
