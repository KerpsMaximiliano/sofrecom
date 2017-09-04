using Sofco.Model.Models.Admin;

namespace Sofco.Model.Relationships
{
    public class RoleFunctionality
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int FunctionalityId { get; set; }
        public Functionality Functionality { get; set; }
    }
}
