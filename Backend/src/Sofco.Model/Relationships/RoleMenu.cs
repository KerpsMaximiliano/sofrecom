using Sofco.Model.Models;

namespace Sofco.Model.Relationships
{
    public class RoleMenu
    {
        public int RoleId { get; set; }
        public Role Role { get; set; }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }
    }
}
