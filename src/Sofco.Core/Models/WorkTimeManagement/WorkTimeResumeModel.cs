namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeResumeModel
    {
        public int HoursApproved { get; set; }

        public int HoursRejected { get; set; }

        public int HoursPending { get; set; }

        public int HoursPendingApproved { get; set; }

        public int Total => HoursApproved + HoursRejected + HoursPending + HoursPendingApproved;

        public int BusinessHours { get; set; }

        public int HoursUntilToday { get; set; }
    }
}
