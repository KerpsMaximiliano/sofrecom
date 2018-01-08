using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using Sofco.Core.DAL;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork unitOfWork;
        
        public EmployeeService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ICollection<Employee> GetAll()
        {
            return unitOfWork.EmployeeRepository.GetAll();
        }

        public Response<Employee> GetById(int id)
        {
            var response = new Response<Employee>();

            response.Data = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            return response;
        }

        public ICollection<EmployeeSyncAction> GetNews()
        {
            return unitOfWork.EmployeeSyncActionRepository.GetAll();
        }
    }
}
