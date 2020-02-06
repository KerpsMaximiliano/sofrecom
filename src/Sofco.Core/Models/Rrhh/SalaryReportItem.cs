using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.Rrhh
{
    public class SalaryReportItem
    {
        public int EmployeeNumber { get; set; }

        public string Name { get; set; }

        public string Manager { get; set; }

        public string Office { get; set; }

        public string Profile { get; set; }

        public string Seniority { get; set; }

        public string Technology { get; set; }

        public DateTime StartDate { get; set; }

        public int Antique { get; set; }

        public IList<MonthSalary> Months { get; set; }
    }

    public class MonthSalary
    {
        public string Display { get; set; }

        public decimal Value { get; set; }
    }
}
