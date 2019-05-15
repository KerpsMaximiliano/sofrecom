using System;

namespace Sofco.Domain.Crm
{
    public class CrmInvoicingMilestone
    {
        public decimal AmountOriginal { get; set; }

        public decimal BaseAmountOriginal { get; set; }

        public decimal BaseAmount { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string Money { get; set; }

        public Guid MoneyId { get; set; }

        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        public int Month { get; set; }

        public string Status { get; set; }

        public int StatusCode { get; set; }

        public Guid OpportunityId { get; set; }

        public string OpportunityName { get; set; }

        public string PurchaseOrder { get; set; }
    }
}
