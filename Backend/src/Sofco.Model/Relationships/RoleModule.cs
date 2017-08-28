using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.Model.Relationships
{
    public class RoleModule
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }
}
