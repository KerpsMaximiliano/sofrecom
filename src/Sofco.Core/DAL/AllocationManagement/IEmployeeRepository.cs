using System.Collections.Generic;
using Sofco.Model.Models.TimeManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository
    {
        ICollection<Employee> GetAll();

        bool Exist(int employeeId);

        List<Employee> Save(List<Employee> employees);
    }
}
