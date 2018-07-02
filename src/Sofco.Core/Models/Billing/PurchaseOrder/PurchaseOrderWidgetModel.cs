namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderWidgetModel
    {
        public string PurchaseOrder { get; set; }

        public decimal Balance { get; set; }

        public decimal BillingPendingAmmount { get; set; }

        public decimal CashPendingAmmount { get; set; }

        public decimal AmmountCashed { get; set; }

        public string Currency { get; set; }
    }
}
