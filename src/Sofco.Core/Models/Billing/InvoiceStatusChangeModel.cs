using Sofco.Model.Enums;

namespace Sofco.Core.Models.Billing
{
    public class InvoiceStatusChangeModel : StatusChangeModel
    {
        public InvoiceStatus Status { get; set; }

        public string InvoiceNumber { get; set; }
    }
}
