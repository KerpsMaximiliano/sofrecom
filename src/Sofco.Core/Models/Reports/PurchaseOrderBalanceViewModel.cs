using System.Collections.Generic;

namespace Sofco.Core.Models.Reports
{
    public class PurchaseOrderBalanceViewModel
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public string Number { get; set; }

        public string ClientExternalName { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyText { get; set; }

        public decimal Ammount { get; set; }

        public int StatusId { get; set; }

        public string StatusText { get; set; }

        public decimal Balance { get; set; }

        public List<PurchaseOrderBalanceDetailViewModel> Details { get; set; }
    }
}
