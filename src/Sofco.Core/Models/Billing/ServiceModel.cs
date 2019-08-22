using Sofco.Domain.Models.Billing;

namespace Sofco.Core.Models.Billing
{
    public class ServiceModel : Service
    {
        public ServiceModel(Service service)
        {
            this.AccountId = service.AccountId;
            this.AccountName = service.AccountName;
            this.Active = service.Active;
            this.Analytic = service.Analytic;
            this.CrmId = service.CrmId;
            this.EndDate = service.EndDate;
            this.Id = service.Id;
            this.Industry = service.Industry;
            this.Manager = service.Manager;
            this.ManagerId = service.ManagerId;
            this.Name = service.Name;
            this.Description = service.Description;
            this.ServiceType = service.ServiceType;
            this.ServiceTypeId = service.ServiceTypeId;
            this.SolutionType = service.SolutionType;
            this.SolutionTypeId = service.SolutionTypeId;
            this.StartDate = service.StartDate;
            this.TechnologyType = service.TechnologyType;
            this.TechnologyTypeId = service.TechnologyTypeId;
        }

        public string Proposals { get; set; }
    }
}
