using Sofco.Domain.Models.Admin;
using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailResource : BaseEntity
    {
        public DateTime Month { get; set; }
        public float Cost { get; set; }

        public int TypeId { get; set; }
        public CostDetailResourceType Type { get; set; }
        
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
