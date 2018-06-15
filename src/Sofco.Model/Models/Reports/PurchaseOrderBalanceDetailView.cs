using System;
using Sofco.Model.Enums;

namespace Sofco.Model.Models.Reports
{
    public class PurchaseOrderBalanceDetailView
    {
        public int Id { get; set; }

        public int SolfacId { get; set; }

        public string Description { get; set; }

        public DateTime UpdatedDate { get; set; }

        public decimal Total { get; set; }

        public int CurrencyId { get; set; }

        public string CurrencyText { get; set; }

        public SolfacStatus Status { get; set; }

        public int PurchaseOrderId { get; set; }

        public string Analytic { get; set; }

        public string AnalyticName { get; set; }
    }
}
