namespace Sofco.Core.Models.WorkTimeManagement
{
    public class ReportParams
    {
        public int CloseMonthId { get; set; }

        public int? ManagerId { get; set; }

        public int? AnalyticId { get; set; }

        public int? EmployeeId { get; set; }

        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }

        public bool ExportTigerVisible { get; set; }
    }
}
