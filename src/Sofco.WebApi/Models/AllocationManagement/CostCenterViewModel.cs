using Sofco.Model.Models.AllocationManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class CostCenterViewModel
    {
        public CostCenterViewModel(CostCenter domain)
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