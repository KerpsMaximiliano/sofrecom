using System;
using System.Linq;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Models.AllocationManagement
{
    public class AnalyticModel : BaseEntity
    {
        public AnalyticModel()
        {
        }

        public AnalyticModel(Analytic domain)
        {
            Id = domain.Id;
            Title = domain.Title;
            Name = domain.Name;
            ClientExternalId = domain.ClientExternalId;
            ClientExternalName = domain.ClientExternalName;
            Service = domain.Service;
            ServiceId = domain.ServiceId;
            SoftwareLawId = domain.SoftwareLawId;
            ActivityId = domain.ActivityId;
            StartDateContract = domain.StartDateContract;
            EndDateContract = domain.EndDateContract;
            CommercialManagerId = domain.CommercialManagerId;
            SectorId = domain.SectorId;
            ManagerId = domain.ManagerId;
            Proposal = domain.Proposal;
            SolutionId = domain.SolutionId;
            TechnologyId = domain.TechnologyId;
            ClientGroupId = domain.ClientGroupId;
            ServiceTypeId = domain.ServiceTypeId;

            if (!string.IsNullOrWhiteSpace(domain.UsersQv))
            {
                UsersQv = domain.UsersQv.Split(';');
            }

            CostCenterId = domain.CostCenterId;
            Status = domain.Status;
            CreationDate = domain.CreationDate;
        }

        public string Title { get; set; }

        public string Name { get; set; }

        public int CostCenterId { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public string ContractNumber { get; set; }

        public string Service { get; set; }

        public string ServiceId { get; set; }

        public int? SoftwareLawId { get; set; }

        public int? ActivityId { get; set; }

        public DateTime StartDateContract { get; set; }

        public DateTime EndDateContract { get; set; }

        public int? CommercialManagerId { get; set; }

        public int SectorId { get; set; }

        public int? ManagerId { get; set; }

        public string Proposal { get; set; }

        public int? SolutionId { get; set; }

        public int? TechnologyId { get; set; }

        public DateTime CreationDate { get; set; }

        public int? ClientGroupId { get; set; }

        public int? ServiceTypeId { get; set; }

        public string[] UsersQv { get; set; }

        public AnalyticStatus Status { get; set; }

        public virtual Analytic CreateDomain()
        {
            var domain = new Analytic();

            FillData(domain);

            domain.CreationDate = DateTime.UtcNow;
            domain.Status = AnalyticStatus.Open;

            return domain;
        }

        protected void FillData(Analytic domain)
        {
            domain.Id = Id;
            domain.Title = Title;
            domain.Name = Name;
            domain.ClientExternalId = ClientExternalId;
            domain.ServiceId = ServiceId;
            domain.ClientExternalName = string.IsNullOrWhiteSpace(ClientExternalId) ? "No Aplica" : ClientExternalName;
            domain.Service = string.IsNullOrWhiteSpace(ServiceId) ? "No Aplica" : Service;
            domain.SoftwareLawId = SoftwareLawId;
            domain.ActivityId = ActivityId;
            domain.StartDateContract = StartDateContract.Date;
            domain.EndDateContract = EndDateContract.Date;
            domain.CommercialManagerId = CommercialManagerId;
            domain.SectorId = SectorId;
            domain.ManagerId = ManagerId;
            domain.Proposal = Proposal;
            domain.SolutionId = SolutionId;
            domain.TechnologyId = TechnologyId;
            domain.ClientGroupId = ClientGroupId;
            domain.ServiceTypeId = ServiceTypeId;

            if (UsersQv != null && UsersQv.Any())
            {
                domain.UsersQv = string.Join(";", UsersQv);
            }

            domain.CostCenterId = CostCenterId;
        }

        public Analytic CreateDomainDaf()
        {
            var domain = new Analytic
            {
                Id = Id,
                SoftwareLawId = SoftwareLawId,
                ActivityId = ActivityId
            };

            return domain;
        }
    }
}
