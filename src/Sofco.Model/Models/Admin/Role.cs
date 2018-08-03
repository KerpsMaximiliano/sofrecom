using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Relationships;

namespace Sofco.Domain.Models.Admin
{
    public class Role : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Description { get; set; }

        public IList<Group> Groups { get; set; }

        public IList<RoleFunctionality> RoleFunctionality { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Code { get; set; }

        public void ApplyTo(Role role)
        {
            role.Id = Id;
            role.Description = Description;
            role.Active = Active;
        }
    }
}
