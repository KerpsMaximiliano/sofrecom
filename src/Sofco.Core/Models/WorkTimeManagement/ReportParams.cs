using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class ReportParams
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int? ManagerId { get; set; }

        public string ClientId { get; set; }

        public int? AnalyticId { get; set; }

        public int? EmployeeId { get; set; }
    }
}
