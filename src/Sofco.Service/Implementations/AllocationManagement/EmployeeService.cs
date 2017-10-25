using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;
using Sofco.Core.DAL.AllocationManagement;

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
    }
}
