using System;
using Sofco.Model.Models.Billing;

namespace Sofco.Model.Models.Common
{
    public class File : BaseEntity
    {
        public Guid InternalFileName { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public DateTime CreationDate { get; set; }

        public string CreatedUser { get; set; }
    }
}
