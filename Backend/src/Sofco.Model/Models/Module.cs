using Sofco.Model.Interfaces;
using Sofco.Model.Relationships;
using System;
using System.Collections.Generic;

namespace Sofco.Model.Models
{
    public class Module : BaseEntity, IAuditDates, ILogicalDelete
    {
        public string Description { get; set; }
        public bool Active { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime StartDate { get; set; }

        public IList<RoleModuleFunctionality> RoleModuleFunctionality { get; set; }
    }
}
