using Sofco.Common.Domains;

namespace Sofco.Model.Models.AllocationManagement
{
    public class CostCenter : BaseEntity, ILogicalDelete
    {
        public int Code { get; set; }
        public string Letter { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
