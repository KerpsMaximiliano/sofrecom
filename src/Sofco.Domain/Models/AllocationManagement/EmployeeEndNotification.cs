using System;
using Sofco.Common.Domains;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class EmployeeEndNotification : BaseEntity, IEntityDate
    {
        public int EmployeeId { get; set; }

        public int ApplicantUserId { get; set; }

        public string Recipients { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? Modified { get; set; }
    }
}
