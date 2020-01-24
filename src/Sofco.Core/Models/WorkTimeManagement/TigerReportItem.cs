using System.Collections.Generic;
using System.Globalization;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class TigerReportItem
    {
        public TigerReportItem(string employeeNumber, decimal realPercentage, int? costCenter, string activity, string analytic)
        {
            EmployeeNumber = employeeNumber;
            Percentage = string.Format("{0:0.00}", realPercentage).Replace(',','.');
            CostCenter = costCenter.GetValueOrDefault().ToString();
            Activity = string.IsNullOrWhiteSpace(activity) ? "012" : activity;
            Analytic = analytic.Contains("-") ? analytic.Split('-')[1] : analytic;
        }

        public int Id { get; set; }

        public string EmployeeNumber { get; set; }

        public string Percentage { get; set; }

        public string CostCenter { get; set; }

        public string Analytic { get; set; }

        public string Activity { get; set; }

        public int AllocationId { get; set; }

        private const string Column5 = "0";

        private const string Column6 = "01";

        private const string Column8 = "000";

        public string GetLine()
        {
            return $"{EmployeeNumber};{Percentage};{CostCenter};{Analytic};{Column5};{Column6};{Activity};{Column8}";
        }
    }
}

