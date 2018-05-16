using System.Collections.Generic;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Model.Utils
{
    public class EmployeeEndReason : Option
    {
        public IList<Employee> Employees { get; set; }
    }
}
