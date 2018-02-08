using Sofco.Model.Models.AllocationManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class AddCostCenterViewModel
    {
        public int? Code { get; set; }

        public string Letter { get; set; }

        public string Description { get; set; }

        public CostCenter CreateDomain()
        {
            var domain = new CostCenter
            {
                Code = Code.GetValueOrDefault(),
                Letter = Letter,
                Description = Description,
                Active = true
            };

            return domain;
        }
    }

    public class EditCostCenterViewModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
