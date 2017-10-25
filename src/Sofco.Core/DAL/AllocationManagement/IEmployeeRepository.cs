using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository
    {
        ICollection<Employee> GetAll();
        bool Exist(int employeeId);
    }
}
