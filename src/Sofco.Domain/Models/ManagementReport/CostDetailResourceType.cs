using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailResourceType : BaseEntity
    {
        public string Name { get; set; }
        public bool Default { get; set; }

        public ICollection<CostDetail> CostDetail { get; set; }
    }
}
