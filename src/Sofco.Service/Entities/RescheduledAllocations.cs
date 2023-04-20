using System;

namespace Sofco.Service.Implementations.Entities
{
    public class RescheduledAllocation
    {
        public DateTime Date { get; set; }

        public decimal Percentage { get; set; }

        public int AnalyticId { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeEmail { get; set; }
        public decimal RemainBusinessHours { get; set; }
        
    }
}
