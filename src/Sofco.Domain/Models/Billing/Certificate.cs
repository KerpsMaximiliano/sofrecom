﻿using System;
using System.Collections.Generic;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Relationships;

namespace Sofco.Domain.Models.Billing
{
    public class Certificate : BaseEntity
    {
        public string Name { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public int Year { get; set; }

        public DateTime UpdateDate { get; set; }

        public string UpdateByUser { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }

        public ICollection<SolfacCertificate> SolfacCertificates { get; set; }
    }
}
