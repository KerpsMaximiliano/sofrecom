using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Model.Interfaces;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Billing;
using Sofco.Model.Models.Rrhh;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Relationships;
using Sofco.Model.Utils;

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

        public string ExternalManagerId { get; set; }

        public ICollection<Analytic> Analytics2 { get; set; }
        public ICollection<Analytic> Analytics3 { get; set; }

        public ICollection<License> Licenses { get; set; }

        public ICollection<WorkTime> WorkTimes1 { get; set; }

        public ICollection<Area> Areas { get; set; }

        public ICollection<Sector> Sectors { get; set; }

        public ICollection<Employee> Employees { get; set; }
    }
}
