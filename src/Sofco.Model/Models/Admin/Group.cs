using System;
using System.Collections.Generic;
using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using Sofco.Common.Domains;

namespace Sofco.Model.Models.Admin
{
    public class Group : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public Role Role { get; set; }

        public IList<UserGroup> UserGroups { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }
    }
}
