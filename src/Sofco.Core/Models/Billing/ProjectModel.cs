using Sofco.Domain.Models.Billing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Models.Billing
{
    public class ProjectModel
    {
        public ProjectModel(Project project)
        {
            Id = project.Id;
            CrmId = project.CrmId;
            Name = project.Name;
            AccountId = project.AccountId;
            ServiceId = project.ServiceId;
            StartDate = project.StartDate;
            EndDate = project.EndDate;
            Incomes = project.Incomes;
            RealIncomes = project.RealIncomes;
            OpportunityId = project.OpportunityId;
            OpportunityName = project.OpportunityName;
            OpportunityNumber = project.OpportunityNumber;
            TotalAmmount = project.TotalAmmount;
            Currency = project.Currency;
            CurrencyId = project.CurrencyId;
            Remito = project.Remito;
            Integrator = project.Integrator;
            IntegratorId = project.IntegratorId;
            Active = project.Active;
            PrincipalContactId = project.PrincipalContactId;
            PrincipalContactName = project.PrincipalContactName;
            PrincipalContactEmail = project.PrincipalContactEmail;
        }
        public int Id { get; set; }
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
        public Guid? PrincipalContactId { get; set; }
        public string PrincipalContactName { get; set; }
        public string PrincipalContactEmail { get; set; }
        public decimal HitosBilled { get; set; }
        public decimal HitosPending { get; set; }
    }
}
