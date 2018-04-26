namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorktimeHoursApprovedParams : WorktimeHoursPendingParams
    {
        public int? Month { get; set; }

        public int? Year { get; set; }
    }

    public class WorktimeHoursPendingParams
    {
        public int? EmployeeId { get; set; }

        public int? AnalyticId { get; set; }
    }
}
