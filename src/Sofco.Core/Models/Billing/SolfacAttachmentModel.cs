using System;
using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class SolfacAttachmentModel
    {
        public SolfacAttachmentModel(SolfacAttachment solfacAttachment)
        {
            Id = solfacAttachment.Id;
            Name = solfacAttachment.Name;
            CreationDate = solfacAttachment.CreationDate;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
