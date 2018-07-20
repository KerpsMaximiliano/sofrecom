namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderAmmountDetailModel
    {
        public int CurrencyId { get; set; }

        public string CurrencyDescription { get; set; }

        public decimal Balance { get; set; }

        public decimal Ammount { get; set; }

        public decimal Adjustment { get; set; }

        public bool Enable { get; set; }
    }
}
