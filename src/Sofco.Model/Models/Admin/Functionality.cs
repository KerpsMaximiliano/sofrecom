using System.Collections.Generic;
using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;

namespace Sofco.Model.Models.Admin
{
    public class Functionality : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public string Code { get; set; }

        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public IList<RoleFunctionality> RoleFunctionality { get; set; }
    }
}
