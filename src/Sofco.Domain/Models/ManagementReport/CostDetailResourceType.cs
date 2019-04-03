using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailResourceType : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<CostDetailResource> Resources { get; set; }
    }
}
