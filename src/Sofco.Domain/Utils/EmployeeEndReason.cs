using System.Collections.Generic;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Utils
{
    public class EmployeeEndReason : Option
    {
        public IList<Employee> Employees { get; set; }
    }
}
