using System.Collections.Generic;

namespace Sofco.Core.Models.AllocationManagement
{
    public class AllocationReportModel
    {
        public AllocationReportModel()
        {
            this.Rows = new List<AllocationReportRow>();
            this.MonthsHeader = new List<string>();
        }

        public IList<AllocationReportRow> Rows { get; set; }
        public IList<string> MonthsHeader { get; set; }
    }

    public class AllocationReportRow
    {
        public AllocationReportRow()
        {
            Months = new List<AllocationMonthReport>();
        }

        public string Analytic { get; set; }

        public string ResourceName { get; set; }

        public string Profile { get; set; }

        public string Seniority { get; set; }

        public string Technology { get; set; }

        public decimal Percentage { get; set; }

        public string Manager { get; set; }

        public IList<AllocationMonthReport> Months { get; set; }

        public string EmployeeNumber { get; set; }
    }

    public class AllocationMonthReport
    {
        public string MonthYear { get; set; }

        public decimal Percentage { get; set; }
    }
}
