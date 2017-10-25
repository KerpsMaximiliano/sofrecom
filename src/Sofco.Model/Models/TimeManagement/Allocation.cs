using System;

namespace Sofco.Model.Models.TimeManagement
{
    public class Allocation : BaseEntity
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Percentage { get; set; }

        public int AnalyticId { get; set; }
        public Analytic Analytic { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
