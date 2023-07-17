using Sofco.Core.Services.Jobs;
using Sofco.WebJob.Jobs.Interfaces;

namespace Sofco.WebJob.Jobs
{
    public class EmployeeWorkTimesAddJob : IEmployeeWorkTimesAddJob
    {
        private readonly IEmployeeWorkTimesAddJobService employeeWorkTimesAddJob;

        public EmployeeWorkTimesAddJob(IEmployeeWorkTimesAddJobService employeeWorkTimesAddJob)
        {
            this.employeeWorkTimesAddJob = employeeWorkTimesAddJob;
        }

        public void Execute()
        {
            employeeWorkTimesAddJob.Run();
        }
    }
}
