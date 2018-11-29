using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlParams
    {
        public int? AnalyticId { get; set; }

        public int? CloseMonthId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
