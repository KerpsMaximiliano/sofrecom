using System;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class ServiceUpdateJobService : IServiceUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;
        private readonly ILogMailer<ServiceUpdateJobService> logger;
        private readonly IServiceData serviceData;

        public ServiceUpdateJobService(IUnitOfWork unitOfWork,
            ICrmHttpClient client,
            IServiceData serviceData,
            IOptions<CrmConfig> crmOptions,
            ILogMailer<ServiceUpdateJobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.client = client;
            crmConfig = crmOptions.Value;
            this.logger = logger;
            this.serviceData = serviceData;
        }

        public void Execute()
        {
            var url = $"{crmConfig.Url}/api/service";

            var result = client.GetMany<CrmService>(url);

            unitOfWork.BeginTransaction();

            foreach (var crmService in result.Data)
            {
                var service = unitOfWork.ServiceRepository.GetByIdCrm(crmService.Id);

                if (service == null)
                    Create(crmService);
                else
                    Update(crmService, service);
            }

            try
            {
                unitOfWork.Commit();
                serviceData.ClearKeys();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmService crmService, Model.Models.Billing.Service service)
        {
            FillData(service, crmService);

            try
            {
                unitOfWork.ServiceRepository.Update(service);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error modificacion cliente: {crmService.Nombre}", e);
            }
        }

        private void Create(CrmService crmService)
        {
            var service = new Model.Models.Billing.Service();

            FillData(service, crmService);

            try
            {
                unitOfWork.ServiceRepository.Insert(service);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error alta cliente: {crmService.Nombre}", e);
            }
        }

        private void FillData(Model.Models.Billing.Service service, CrmService crmService)
        {
            service.CrmId = crmService.Id;
            service.Name = crmService.Nombre;
            service.AccountId = crmService.AccountId;
            service.AccountName = crmService.AccountName;
            service.Industry = crmService.Industry;
            service.StartDate = crmService.StartDate;
            service.EndDate = crmService.EndDate;
            service.Manager = crmService.Manager;
            service.ManagerId = crmService.ManagerId;
            service.ServiceType = crmService.ServiceType;
            service.ServiceTypeId = crmService.ServiceTypeId;
            service.SolutionType = crmService.SolutionType;
            service.SolutionTypeId = crmService.SolutionTypeId;
            service.TechnologyType = crmService.TechnologyType;
            service.TechnologyTypeId = crmService.TechnologyTypeId;
            service.Analytic = crmService.Analytic;
            service.Active = true;
        }
    }
}
