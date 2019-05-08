using System;

namespace Sofco.Domain.Models.ManagementReport
{
    public class ContratedDetail : BaseEntity
    {
        public int CostDetailId { get; set; }
        public CostDetail CostDetail { get; set; }

        public string Name { get; set; }

        public DateTime MonthYear { get; set; }

        public decimal Insurance { get; set; }

        public decimal Honorary { get; set; }
    }
}
