using System;
using System.Collections.Generic;

namespace Sofco.Domain.DTO
{
    public class AllocationReportParams
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<int> AnalyticIds { get; set; }

        public int? EmployeeId { get; set; }

        public decimal Percentage { get; set; }

        public bool IncludeStaff { get; set; }

        public bool Unassigned { get; set; }

        public int IncludeAnalyticId { get; set; }

        public decimal? StartPercentage { get; set; }

        public decimal? EndPercentage { get; set; }
    }
}
