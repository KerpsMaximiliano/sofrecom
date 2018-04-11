using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeWorkTimeApprovalQuery
    {
        public Guid CustomerId { get; set; }

        public Guid ServiceId { get; set; }

        public int ApprovalId { get; set; }
    }
}
