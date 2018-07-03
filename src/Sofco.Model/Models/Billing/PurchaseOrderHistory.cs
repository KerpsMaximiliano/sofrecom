using Sofco.Model.Enums;

namespace Sofco.Model.Models.Billing
{
    public class PurchaseOrderHistory : History
    {
        public PurchaseOrderStatus From { get; set; }
        public PurchaseOrderStatus To { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
