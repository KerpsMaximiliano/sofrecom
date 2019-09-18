using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailSubtype : BaseEntity
    {
        public string Name { get; set; }

        public int CostDetailTypeId { get; set; }
        public CostDetailType CostDetailType { get; set; }

        public ICollection<CostDetailOther> CostDetailOther { get; set; }

    }
}
