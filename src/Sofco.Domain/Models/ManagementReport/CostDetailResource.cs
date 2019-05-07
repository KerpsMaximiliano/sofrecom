using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailResource : BaseEntity
    {
        public int CostDetailId { get; set; }

        public CostDetail CostDetail { get; set; }

        public decimal Value { get; set; }

        public decimal Adjustment { get; set; }

        public decimal Charges { get; set; }

        public int EmployeeId { get; set; }
        
        public Employee Employee { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
