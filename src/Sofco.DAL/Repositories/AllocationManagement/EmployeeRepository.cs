using System.Collections.Generic;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Model.Models.TimeManagement;
using System.Linq;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly SofcoContext context;

        public EmployeeRepository(SofcoContext context)
        {
            this.context = context;
        }

        public bool Exist(int employeeId)
        {
            return context.Employees.Any(x => x.Id == employeeId);
        }

        public ICollection<Employee> GetAll()
        {
            return context.Employees.ToList().AsReadOnly();
        }

        public Employee GetById(int id)
        {
            return context.Employees.SingleOrDefault(x => x.Id == id);
        }
    }
}
