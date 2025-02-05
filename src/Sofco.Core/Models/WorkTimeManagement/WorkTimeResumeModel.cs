﻿namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeResumeModel
    {
        public decimal HoursApproved { get; set; }

        public decimal HoursRejected { get; set; }

        public decimal HoursPending { get; set; }

        public decimal HoursPendingApproved { get; set; }

        public decimal Total => HoursApproved + HoursRejected + HoursDraft + HoursPendingApproved + HoursWithLicense;

        public decimal BusinessHours { get; set; }

        public decimal HoursUntilToday { get; set; }

        public decimal HoursWithLicense { get; set; }

        public decimal HoursDraft { get; set; }
    }
}
