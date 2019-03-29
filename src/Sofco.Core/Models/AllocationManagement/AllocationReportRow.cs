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
            Months = new List<AllocationDateReport>();
        }

        public int AnalyticId { get; set; }

        public int EmployeeId { get; set; }

        public string Analytic { get; set; }

        public string ResourceName { get; set; }

        public string Profile { get; set; }

        public string Seniority { get; set; }

        public string Technology { get; set; }

        public decimal Percentage { get; set; }

        public string Manager { get; set; }

        public IList<AllocationDateReport> Months { get; set; }

        public string EmployeeNumber { get; set; }
    }

    public class AllocationDateReport
    {
        public AllocationDateReport()
        {
        }

        public AllocationDateReport(AllocationDateReport x)
        {
            Year = x.Year;
            Month = x.Month;
            Percentage = x.Percentage;
        }

        public int Year { get; set; }

        public int Month { get; set; }

        public decimal Percentage { get; set; }
    }
}
