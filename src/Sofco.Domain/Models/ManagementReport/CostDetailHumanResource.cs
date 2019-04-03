using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailHumanResource : BaseEntity
    {
        public DateTime Month { get; set; }
        public float Cost { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? ModifiedById { get; set; }
        public User ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public int CostDetailId { get; set; }
        public CostDetail CostDetail { get; set; }

    }
}
