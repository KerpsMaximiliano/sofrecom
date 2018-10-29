using System;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorktimeHoursApprovedParams : WorktimeHoursPendingParams
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool FilterByDates { get; set; }
    }

    public class WorktimeHoursPendingParams
    {
        public int? EmployeeId { get; set; }

        public int? AnalyticId { get; set; }
    }
}
