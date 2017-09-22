using System;

namespace Sofco.Model.Models.Billing
{
    public class SolfacAttachment : BaseEntity
    {
        public byte[] File { get; set; }
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }

        public int SolfacId { get; set; }
        public Solfac Solfac { get; set; }
    }
}
