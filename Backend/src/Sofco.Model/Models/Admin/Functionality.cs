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

        public IList<ModuleFunctionality> ModuleFunctionality { get; set; }
    }
}
