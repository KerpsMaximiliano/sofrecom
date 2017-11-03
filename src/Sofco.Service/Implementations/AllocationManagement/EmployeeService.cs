using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        
        public EmployeeService(IEmployeeRepository employeeRepo)
        {
            employeeRepository = employeeRepo;
        }

        public ICollection<Employee> GetAll()
        {
            return employeeRepository.GetAll();
        }

        public Response<Employee> GetById(int id)
        {
            var response = new Response<Employee>();

            response.Data = EmployeeValidationHelper.Find(response, employeeRepository, id);

            return response;
        }
    }
}
