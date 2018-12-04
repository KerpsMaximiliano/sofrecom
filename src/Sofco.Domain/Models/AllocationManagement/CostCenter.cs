using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class CostCenter : BaseEntity, ILogicalDelete
    {
        public int Code { get; set; }
        public string Letter { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public ICollection<Analytic> Analytics { get; set; }
    }
}
