using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using System.Collections.Generic;
using System;

namespace Sofco.Model.Models
{
    public class User : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<UserGroup> UserGroups { get; set; }
    }
}
