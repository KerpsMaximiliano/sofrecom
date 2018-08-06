using System;

namespace Sofco.Domain.DTO
{
    public class AllocationAsignmentParams
    {
        public int AnalyticId { get; set; }
        public int EmployeeId { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? BillingPercentage { get; set; }
        public DateTime? DateSince { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
