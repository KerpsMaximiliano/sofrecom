using Sofco.Model.Models.TimeManagement;
using System.Collections.Generic;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();
    }
}
