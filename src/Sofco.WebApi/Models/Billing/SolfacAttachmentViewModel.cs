using System;
using Sofco.Model.Models.Billing;

namespace Sofco.WebApi.Models.Billing
{
    public class SolfacAttachmentViewModel
    {
        public SolfacAttachmentViewModel(SolfacAttachment solfacAttachment)
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
