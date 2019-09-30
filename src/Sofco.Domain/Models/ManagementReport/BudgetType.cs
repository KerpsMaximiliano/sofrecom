using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class BudgetType : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<CostDetailStaff> CostDetailStaff { get; set; }
        public ICollection<ManagementReport> ManagementReport { get; set; }
        public ICollection<CostDetailResource> CostDetailResource { get; set; }
    }
}
