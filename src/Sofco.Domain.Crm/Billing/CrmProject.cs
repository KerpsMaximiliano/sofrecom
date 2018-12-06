using System;

namespace Sofco.Domain.Crm.Billing
{
    public class CrmProject
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string AccountId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Analytic { get; set; }

        public decimal Incomes { get; set; }

        public decimal RealIncomes { get; set; }

        public string PurchaseOrder { get; set; }

        public string OpportunityId { get; set; }

        public string OpportunityName { get; set; }

        public string OpportunityNumber { get; set; }

        public decimal TotalAmount { get; set; }

        public string ServiceType { get; set; }

        public int ServiceTypeId { get; set; }

        public string SolutionType { get; set; }

        public int SolutionTypeId { get; set; }

        public string TechnologyType { get; set; }
        public int TechnologyTypeId { get; set; }

        public string Currency { get; set; }

        public string CurrencyId { get; set; }

        public bool Remito { get; set; }

        public string Manager { get; set; }

        public string ManagerId { get; set; }

        public string OwnerId { get; set; }

        public string Integrator { get; set; }

        public Guid IntegratorId { get; set; }

        public string ServiceId { get; set; }

        public Guid? PrincipalContactId { get; set; }

        public string PrincipalContactName { get; set; }
    }
}
