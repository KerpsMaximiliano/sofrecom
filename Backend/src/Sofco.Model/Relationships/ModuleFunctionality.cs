using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.Model.Relationships
{
    public class ModuleFunctionality
    {
        public int FunctionalityId { get; set; }
        public Functionality Functionality { get; set; }

        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }
}
