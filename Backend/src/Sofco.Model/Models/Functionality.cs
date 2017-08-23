using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class Functionality : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public string Code { get; set; }

        public IList<ModuleFunctionality> ModuleFunctionality { get; set; }
    }
}
