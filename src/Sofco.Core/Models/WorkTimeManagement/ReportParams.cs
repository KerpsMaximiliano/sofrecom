using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class ReportParams
    {
        public int CloseMonthId { get; set; }

        public IList<int> ManagerId { get; set; }

        public IList<int> AnalyticId { get; set; }

        public int? EmployeeId { get; set; }

        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }

        public bool ExportTigerVisible { get; set; }
    }
}
