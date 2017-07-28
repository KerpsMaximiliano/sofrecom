using Sofco.Model.Interfaces;
using System.Collections.Generic;
using System;
using Sofco.Model.Relationships;

namespace Sofco.Model.Models
{
    public class Group : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public Role Role { get; set; }

        public IList<UserGroup> UserGroups { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public void ApplyTo(Group item)
        {
            item.Id = this.Id;
            item.Description = this.Description;
            item.Active = this.Active;
        }
    }
}
