using Sofco.Domain.Enums;

namespace Sofco.Domain.Models.Billing
{
    public class PurchaseOrderHistory : History
    {
        public PurchaseOrderStatus From { get; set; }
        public PurchaseOrderStatus To { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
