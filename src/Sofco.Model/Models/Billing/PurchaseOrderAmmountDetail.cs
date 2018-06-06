using Sofco.Model.Utils;

namespace Sofco.Model.Models.Billing
{
    public class PurchaseOrderAmmountDetail
    {
        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public int CurrencyId { get; set; }
        public Currency Currency { get; set; }

        public decimal Balance { get; set; }

        public decimal Ammount { get; set; }
    }
}
