using System.Collections.Generic;

namespace Sofco.Core.Models.Billing.PurchaseOrder
{
    public class PurchaseOrderAdjustmentModel
    {
        public string Comments { get; set; }

        public IList<PurchaseOrderAmmountDetailModel> Items { get; set; }
    }

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
