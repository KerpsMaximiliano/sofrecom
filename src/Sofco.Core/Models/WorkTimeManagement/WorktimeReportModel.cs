namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeReportModel
    {
        public string Client { get; set; }

        public string Analytic { get; set; }

        public string Manager { get; set; }

        public string MonthYear { get; set; }

        public decimal Facturability { get; set; }

        public decimal AllocationPercentage { get; set; }

        public decimal HoursMustLoad { get; set; }

        public decimal HoursLoaded { get; set; }

        public string Employee { get; set; }

        public bool Result { get; set; }
    }
}
