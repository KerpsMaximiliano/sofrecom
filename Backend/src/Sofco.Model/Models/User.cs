using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class User : BaseEntity, ILogicalDelete
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public IList<UserGroup> UserGroups { get; set; }
    }
}
