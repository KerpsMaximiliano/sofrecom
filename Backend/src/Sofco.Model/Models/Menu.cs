using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using System;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class Menu : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Description { get; set; }

        public string Url { get; set; }

        public string Code { get; set; }

        public bool Active { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }

        public IList<RoleMenu> RoleMenu { get; set; }
    }
}
