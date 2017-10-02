using Sofco.Model.Enums;

namespace Sofco.Model.Models.Billing
{
    public class InvoiceHistory : History
    {
        public InvoiceStatus StatusFrom { get; set; }
        public InvoiceStatus StatusTo { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }
}
