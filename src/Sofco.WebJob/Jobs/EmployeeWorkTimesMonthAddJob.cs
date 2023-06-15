using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeWorkTimesMonthAddJob : IEmployeeWorkTimesMonthAddJob
    {
        private readonly IEmployeeWorkTimesMonthAddJobService employeeWorkTimesAddJob;

        public EmployeeWorkTimesMonthAddJob(IEmployeeWorkTimesMonthAddJobService employeeWorkTimesAddJob)
        {
            this.employeeWorkTimesAddJob = employeeWorkTimesAddJob;
        }

        public void Execute()
        {
            employeeWorkTimesAddJob.Run();
        }
    }
}
