using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.Rrhh
{
    public class SalaryReportResponse
    {
        public IList<SalaryReportItem> Items { get; set; }

        public IList<string> Months { get; set; }
    }

    public class SalaryReportItem
    {
        public int EmployeeId { get; set; }

        public string EmployeeNumber { get; set; }

        public string Name { get; set; }

        public string Manager { get; set; }

        public string Office { get; set; }

        public string Profile { get; set; }

        public string Seniority { get; set; }

        public string Technology { get; set; }

        public DateTime StartDate { get; set; }

        public double Antique { get; set; }

        public IList<SalaryValueItem> Values { get; set; }
    }

    public class SalaryValueItem
    {
        public decimal Value { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }
    }
}
