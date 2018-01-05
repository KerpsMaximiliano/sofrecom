using System;
using Sofco.Model;
using Sofco.Model.Enums.TimeManagement;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.WebApi.Models.AllocationManagement
{
    public class AnalyticViewModel : BaseEntity
    {
        public string Title { get; set; }

        public string Name { get; set; }

        public int CostCenterId { get; set; }

        public string ClientExternalId { get; set; }

        public string ClientExternalName { get; set; }

        public string ContractNumber { get; set; }

        public string Service { get; set; }

        public int? SoftwareLawId { get; set; }

        public int? ActivityId { get; set; }

        public DateTime StartDateContract { get; set; }

        public DateTime EndDateContract { get; set; }

        public int? CommercialManagerId { get; set; }

        public int DirectorId { get; set; }

        public int? ManagerId { get; set; }

        public bool? EvalProp { get; set; }

        public string Proposal { get; set; }

        public int? CurrencyId { get; set; }

        public string AmountEarned { get; set; }

        public string AmountProject { get; set; }

        public int? SolutionId { get; set; }

        public int? TechnologyId { get; set; }

        public string Description { get; set; }

        public int? ProductId { get; set; }

        public DateTime CreationDate { get; set; }

        public string ClientProjectTfs { get; set; }

        public int? ClientGroupId { get; set; }

        public int? PurchaseOrderId { get; set; }

        public int? ServiceTypeId { get; set; }

        public bool BugsAccess { get; set; }

        public string UsersQv { get; set; }

        public Analytic CreateDomain()
        {
            var domain = new Analytic();

            domain.Title = Title;
            domain.Name = Name;
            domain.ClientExternalId = ClientExternalId;
            domain.ClientExternalName = ClientExternalName;
            domain.ContractNumber = ContractNumber;
            domain.Service = Service;
            domain.SoftwareLawId = SoftwareLawId;
            domain.ActivityId = ActivityId;
            domain.StartDateContract = StartDateContract.Date;
            domain.EndDateContract = EndDateContract.Date;
            domain.CommercialManagerId = CommercialManagerId;
            domain.DirectorId = DirectorId;
            domain.ManagerId = ManagerId;
            domain.EvalProp = EvalProp;
            domain.Proposal = Proposal;
            domain.CurrencyId = CurrencyId;
            domain.AmountEarned = AmountEarned;
            domain.AmountProject = AmountProject;
            domain.SolutionId = SolutionId;
            domain.TechnologyId = TechnologyId;
            domain.Description = Description;
            domain.ProductId = ProductId;
            domain.ClientProjectTfs = ClientProjectTfs;
            domain.ClientGroupId = ClientGroupId;
            domain.PurchaseOrderId = PurchaseOrderId;
            domain.ServiceTypeId = ServiceTypeId;
            domain.BugsAccess = BugsAccess;
            domain.UsersQv = UsersQv;
            domain.CostCenterId = CostCenterId;

            domain.CreationDate = DateTime.UtcNow;
            domain.Status = AnalyticStatus.Open;

            return domain;
        }
    }
}
