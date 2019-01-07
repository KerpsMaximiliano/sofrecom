using System;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeEndNotificationParameters
    {
        public int? EmployeeId { get; set; }

        public int? ApplicantEmployeeId { get; set; }

        public int? ApplicantUserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
