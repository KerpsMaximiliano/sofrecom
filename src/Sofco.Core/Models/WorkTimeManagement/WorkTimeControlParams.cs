using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeControlParams
    {
        public IList<int> AnalyticId { get; set; }

        public int? CloseMonthId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
