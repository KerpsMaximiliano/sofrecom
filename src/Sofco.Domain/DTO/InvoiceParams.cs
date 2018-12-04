using Sofco.Domain.Enums;
using System;

namespace Sofco.Domain.DTO
{
    public class InvoiceParams
    {
        public string CustomerId { get; set; }
        public string ServiceId { get; set; }
        public string ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public InvoiceStatus? Status { get; set; }
        public int userApplicantId { get; set; }
        public DateTime? DateSince { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
