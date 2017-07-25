using Sofco.Model.Interfaces;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class UserGroup : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public Role Role { get; set; }

        public IList<User> Users { get; set; }

        public void ApplyTo(UserGroup item)
        {
            item.Id = this.Id;
            item.Description = this.Description;
            item.Active = this.Active;
        }
    }
}
