using Sofco.Core.DAL;
using Sofco.Core.Services.Jobs;
using Sofco.DAL;

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
            unitOfWork.EmployeeRepository.ResetAllPaternityDays();
            unitOfWork.EmployeeRepository.ResetAllBirthdayDays();
            unitOfWork.EmployeeRepository.ResetAllFlexDays();
        }
    }
}
