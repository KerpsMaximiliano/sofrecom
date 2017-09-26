using Sofco.Model.Enums;

namespace Sofco.WebApi.Models.Billing
{
    public class StatusChangeViewModel
    {
        public int UserId { get; set; }
        public string Comment { get; set; }
    }

    public class InvoiceStatusChangeViewModel : StatusChangeViewModel
    {
        public InvoiceStatus Status { get; set; }
        public string InvoiceNumber { get; set; }
    }

    public class SolfacStatusChangeViewModel : StatusChangeViewModel
    {
        public SolfacStatus Status { get; set; }
        public string InvoiceCode { get; set; }
    }
}
