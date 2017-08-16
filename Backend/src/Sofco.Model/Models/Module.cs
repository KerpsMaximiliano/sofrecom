using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class Module : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public string Code { get; set; }

        public Menu Menu { get; set; }

        public IList<RoleModuleFunctionality> RoleModuleFunctionality { get; set; }
    }
}
