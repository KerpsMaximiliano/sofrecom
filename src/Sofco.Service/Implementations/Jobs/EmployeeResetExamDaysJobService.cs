using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeResetExamDaysJobService : IEmployeeResetExamDaysJobService
    {
        private readonly IUnitOfWork unitOfWork;

        public EmployeeResetExamDaysJobService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
         
        public void ResetExamDays()
        {
            unitOfWork.EmployeeRepository.ResetAllExamDays();
        }
    }
}
