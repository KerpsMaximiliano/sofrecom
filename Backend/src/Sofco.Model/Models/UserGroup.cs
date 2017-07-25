using Sofco.Model.Interfaces;
using System.Collections.Generic;
using System;

namespace Sofco.Model.Models
{
    public class UserGroup : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public Role Role { get; set; }

        public IList<User> Users { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public void ApplyTo(UserGroup item)
        {
            item.Id = this.Id;
            item.Description = this.Description;
        }
    }
}
