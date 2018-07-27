using System;

namespace Sofco.Model.Models.Billing
{
    public class Project : BaseEntity
    {
        public string CrmId { get; set; }

        public string Name { get; set; }

        public string AccountId { get; set; }

        public string ServiceId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Incomes { get; set; }

        public decimal RealIncomes { get; set; }

        public string OpportunityId { get; set; }

        public string OpportunityName { get; set; }

        public string OpportunityNumber { get; set; }

        public decimal TotalAmmount { get; set; }

        public string Currency { get; set; }

        public string CurrencyId { get; set; }

        public bool Remito { get; set; }

        public string Integrator { get; set; }

        public string IntegratorId { get; set; }

        public bool Active { get; set; }
    }
}
