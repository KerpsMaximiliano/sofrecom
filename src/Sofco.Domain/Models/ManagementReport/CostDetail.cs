using Sofco.Domain.Models.AllocationManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetail : BaseEntity
    {
        public int IdAnalytic { get; set; }
        public Analytic Analytic { get; set; }

        public List<CostDetailResource> Resources { get; set; }

        public List<CostDetailHumanResource> Employees { get; set; }
    }
}
