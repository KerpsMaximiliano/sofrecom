using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeProfileHistoryModel
    {
        public string Fields { get; set; }

        public string Values { get; set; }

        public Employee Employee { get; set; }

        public Employee EmployeePrevious { get; set; }

        public string DateTime { get; set; }
    }
}
