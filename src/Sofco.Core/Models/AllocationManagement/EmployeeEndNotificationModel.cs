using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.AllocationManagement
{
    public class EmployeeEndNotificationModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeNumber { get; set; }

        public IList<string> Recipients { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ApplicantUserId { get; set; }

        public string ApplicantName { get; set; }

        public string UserName { get; set; }
    }
}
