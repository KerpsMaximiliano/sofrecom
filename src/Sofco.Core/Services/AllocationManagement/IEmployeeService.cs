using Sofco.Model.Models.TimeManagement;
using Sofco.Model.Utils;
using System.Collections.Generic;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();
        Response<Employee> GetById(int id);
    }
}
