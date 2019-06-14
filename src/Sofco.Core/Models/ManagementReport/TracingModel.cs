using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.ManagementReport
{
    public class TracingModel
    {
        public string AnalyticName { get; set; }
        public List<MonthTracing> MonthsTracking { get; set; }
    }

    public class MonthTracing
    {
        public string Display { get; set; }
        public string PercentageExpectedTotal { get; set; }
        public string PercentageToEnd { get; set; }
        public DateTime MonthYear { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

}
