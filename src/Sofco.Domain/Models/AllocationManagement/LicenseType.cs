using System.Collections.Generic;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Domain.Models.AllocationManagement
{
    public class LicenseType : BaseEntity
    {
        public string Description { get; set; }

        public int Days { get; set; }

        public bool WithPayment { get; set; }

        public int TaskId { get; set; }

        public bool CertificateRequired { get; set; }

        public bool Active { get; set; }

        public string LabelKey { get; set; }

        public ICollection<License> Licenses { get; set; }
    }
}
