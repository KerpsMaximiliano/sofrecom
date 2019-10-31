using System;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class Allocation : BaseEntity
    {
        public DateTime StartDate { get; set; }

        public decimal Percentage { get; set; }

        public int AnalyticId { get; set; }

        public Analytic Analytic { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }

        public DateTime ReleaseDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
    }
}
