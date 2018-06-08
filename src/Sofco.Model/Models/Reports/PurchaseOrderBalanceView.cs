using Sofco.Model.Enums;

namespace Sofco.Model.Models.Reports
{
    public class PurchaseOrderBalanceView
    {
        public int Id { get; set; }

        public string ClientExternalId { get; set; }

        public int PurchaseOrderId { get; set; }

        public string Number { get; set; }

        public string ClientExternalName { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyText { get; set; }

        public decimal? Ammount { get; set; }

        public PurchaseOrderStatus Status { get; set; }

        public decimal? Balance { get; set; }
    }
}
