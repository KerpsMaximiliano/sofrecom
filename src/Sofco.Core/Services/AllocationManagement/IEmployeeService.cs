using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();
        Response<Employee> GetById(int id);
    }
}
