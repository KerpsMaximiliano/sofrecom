using System;
using System.Collections.Generic;
using Sofco.Domain.Relationships;

namespace Sofco.Domain.Models.Common
{
    public class File : BaseEntity
    {
        public Guid InternalFileName { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public DateTime CreationDate { get; set; }

        public string CreatedUser { get; set; }

        public ICollection<LicenseFile> LicenseFiles { get; set; }
    }
}
