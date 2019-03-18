namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderWidgetModel
    {
        public string PurchaseOrder { get; set; }

        public decimal Balance => AdjustmentBalance ?? TotalAmmount - CashPendingAmmount - AmmountCashed;

        public decimal BillingPendingAmmount { get; set; }

        public decimal CashPendingAmmount { get; set; }

        public decimal AmmountCashed { get; set; }

        public string Currency { get; set; }

        public decimal? AdjustmentBalance { get; set; }
        public decimal TotalAmmount { get; set; }
    }
}
