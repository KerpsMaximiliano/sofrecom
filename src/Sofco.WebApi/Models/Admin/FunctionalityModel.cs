using Sofco.Model;
using Sofco.Model.Models.Admin;

namespace Sofco.WebApi.Models.Admin
{
    public class FunctionalityModel : BaseEntity
    {
        public FunctionalityModel(Functionality functionality)
        {
            Id = functionality.Id;
            Description = functionality.Description;
            Active = functionality.Active;
        }

        public string Description { get; set; }

        public bool Active { get; set; }
    }
}
