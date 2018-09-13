using System;

namespace Sofco.Core.Models.Reports
{
    public class PurchaseOrderBalanceDetailViewModel
    {
        public int Id { get; set; }

        public int SolfacId { get; set; }

        public string Description { get; set; }

        public DateTime UpdatedDate { get; set; }

        public decimal Total { get; set; }

        public int StatusId { get; set; }

        public string StatusText { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyText { get; set; }

        public int PurchaseOrderId { get; set; }

        public string Analytic { get; set; }
    }
}
