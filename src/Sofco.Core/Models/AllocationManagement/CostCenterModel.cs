using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class CostCenterModel
    {
        public CostCenterModel(CostCenter domain)
        {
            Id = domain.Id;
            Code = domain.Code;
            Letter = domain.Letter;
            Description = domain.Description;
            Active = domain.Active;
        }

        public int Id { get; set; }

        public int Code { get; set; }

        public string Letter { get; set; }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}
