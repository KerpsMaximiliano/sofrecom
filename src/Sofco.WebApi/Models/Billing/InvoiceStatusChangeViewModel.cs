using Sofco.Model.Enums;

namespace Sofco.WebApi.Models.Billing
{
    public class InvoiceStatusChangeViewModel : StatusChangeViewModel
    {
        public InvoiceStatus Status { get; set; }

        public string InvoiceNumber { get; set; }
    }
}
