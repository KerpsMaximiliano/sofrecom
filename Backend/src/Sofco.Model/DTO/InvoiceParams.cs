using Sofco.Model.Enums;

namespace Sofco.Model.DTO
{
    public class InvoiceParams
    {
        public string CustomerId { get; set; }
        public string ServiceId { get; set; }
        public string ProjectId { get; set; }
        public string InvoiceNumber { get; set; }
        public InvoiceStatus? Status { get; set; }
        public int UserId { get; set; }
    }
}
