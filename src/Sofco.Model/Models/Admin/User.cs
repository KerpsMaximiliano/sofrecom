using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Model.Interfaces;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Billing;
using Sofco.Model.Relationships;

namespace Sofco.Model.Models.Admin
{
    public class User : BaseEntity, ILogicalDelete, IAuditDates
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<UserGroup> UserGroups { get; set; }

        public IList<Solfac> Solfacs { get; set; }

        public IList<Invoice> Invoices { get; set; }

        public ICollection<Analytic> Analytics1 { get; set; }
        public ICollection<Analytic> Analytics2 { get; set; }
        public ICollection<Analytic> Analytics3 { get; set; }
    }
}
