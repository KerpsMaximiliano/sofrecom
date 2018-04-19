using System;

namespace Sofco.Model.DTO
{
    public class AllocationReportParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? AnalyticId { get; set; }

        public int? EmployeeId { get; set; }

        public decimal? Percentage { get; set; }

        public bool IncludeStaff { get; set; }

        public decimal? StartPercentage { get; set; }

        public decimal? EndPercentage { get; set; }
    }
}
