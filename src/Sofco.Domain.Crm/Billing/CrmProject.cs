using System;

namespace Sofco.Domain.Crm.Billing
{
    public class CrmProject
    {
        public string Id { get; set; }

        public string Nombre { get; set; }

        public string AccountId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Analytic { get; set; }

        public decimal Incomes { get; set; }

        public string PurchaseOrder { get; set; }

        public string OpportunityId { get; set; }

        public string OpportunityName { get; set; }

        public decimal TotalAmmount { get; set; }

        public string ServiceType { get; set; }

        public string SolutionType { get; set; }

        public string TechnologyType { get; set; }

        public string Currency { get; set; }

        public string CurrencyId { get; set; }

        public bool Remito { get; set; }

        public string Manager { get; set; }

        public string ManagerId { get; set; }
    }
}
