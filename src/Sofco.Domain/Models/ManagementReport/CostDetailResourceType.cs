using System.Collections.Generic;

namespace Sofco.Domain.Models.ManagementReport
{
    public class CostDetailType : BaseEntity
    {
        public string Name { get; set; }

        public bool Default { get; set; }

        public ICollection<CostDetailOther> CostDetailOthers { get; set; }
    }
}
