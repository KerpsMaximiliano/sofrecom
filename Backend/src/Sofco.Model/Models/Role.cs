using Sofco.Model.Interfaces;
using System.Collections.Generic;
using System;

namespace Sofco.Model.Models
{
    public class Role : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public IList<UserGroup> UserGroups { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
