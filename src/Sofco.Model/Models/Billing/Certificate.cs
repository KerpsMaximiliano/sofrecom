using System;
using System.Collections.Generic;
using Sofco.Model.Models.Common;
using Sofco.Model.Relationships;

namespace Sofco.Model.Models.Billing
{
    public class Certificate : BaseEntity
    {
        public string Name { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public int Year { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateByUser { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }

        public ICollection<SolfacCertificate> SolfacCertificates { get; set; }
    }
}
