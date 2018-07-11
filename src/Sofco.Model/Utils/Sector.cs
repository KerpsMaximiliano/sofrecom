using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Model.Models.Admin;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.Rrhh;

namespace Sofco.Model.Utils
{
    public class Sector : Option, ILogicalDelete
    {
        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<License> Licenses { get; set; }

        public ICollection<Analytic> Analytics { get; set; }

        public int ResponsableUserId { get; set; }

        public User ResponsableUser { get; set; }
    }
}
