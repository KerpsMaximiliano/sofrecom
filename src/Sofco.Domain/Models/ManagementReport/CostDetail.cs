using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using System;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetail : BaseEntity
    {
        public int IdAnalytic { get; set; }
        public Analytic Analytic { get; set; }

        public DateTime MonthYear { get; set; }
        public float Cost { get; set; }

        public int TypeId { get; set; }
        public CostDetailResourceType Type { get; set; }

        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? ModifiedById { get; set; }
        public User ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

    }
}
