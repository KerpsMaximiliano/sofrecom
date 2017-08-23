using Sofco.Model.Models;

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
