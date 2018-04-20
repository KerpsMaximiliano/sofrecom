using System;

namespace Sofco.Model.Models.AllocationManagement
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
    }
}
