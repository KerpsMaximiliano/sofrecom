using Sofco.Model.Interfaces;

namespace Sofco.Model.Models
{
    public class UserGroup : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public Role Role { get; set; }
    }
}
