using Sofco.Model.Interfaces;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class Role : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public string Position { get; set; }

        public bool Active { get; set; }

        public IList<UserGroup> UserGroups { get; set; }
    }
}
