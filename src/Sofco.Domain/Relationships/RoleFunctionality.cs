using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Relationships
{
    public class RoleFunctionality
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int FunctionalityId { get; set; }
        public Functionality Functionality { get; set; }
    }
}
