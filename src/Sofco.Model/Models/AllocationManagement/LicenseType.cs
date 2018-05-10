using System;
using System.Collections.Generic;
using Sofco.Common.Domains;
using Sofco.Model.Models.Rrhh;

namespace Sofco.Model.Models.AllocationManagement
{
    public class LicenseType : BaseEntity
    {
        public string Description { get; set; }

        public int Days { get; set; }

        public bool WithPayment { get; set; }

        public int TaskId { get; set; }

        public ICollection<License> Licenses { get; set; }
    }
}
