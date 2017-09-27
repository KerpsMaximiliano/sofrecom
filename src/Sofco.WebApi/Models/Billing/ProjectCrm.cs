using System;

namespace Sofco.WebApi.Models.Billing
{
    public class ProjectCrm
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Analytic { get; set; }
        public decimal Incomes { get; set; }
        public string PurchaseOrder { get; set; }
        public string OportunityId { get; set; }
        public string OportunityName { get; set; }
        public decimal TotalAmmount { get; set; }
        public string ServiceType { get; set; }
        public string SolutionType { get; set; }
        public string TechnologyType { get; set; }
        public string Currency { get; set; }
        public bool Remito { get; set; }
    }
}
