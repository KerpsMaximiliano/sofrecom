using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class CostDetailMonthModel
    {
        public int AnalyticId { get; set; }
        public List<CostMonth> Employees { get; set; }
        public List<CostMonth> OtherResources { get; set; }
    }

    public class CostMonth
    {
        public int? EmployeeId { get; set; }
        public int TypeId { get; set; }
        public DateTime MonthYear { get; set; }
        public int CostDetailId { get; set; }
        public float? Salary { get; set; }
        public float? Charges { get; set; }
    }
}
