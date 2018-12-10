using System;
using System.Collections.Generic;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class ServiceUpdateJobService : IServiceUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ServiceUpdateJobService> logger;
        private readonly IServiceData serviceData;
        private readonly ICrmServiceService crmServiceService;

        private IList<int> IdsAdded { get; }

        public ServiceUpdateJobService(IUnitOfWork unitOfWork,
            IServiceData serviceData,
            ILogMailer<ServiceUpdateJobService> logger, 
            ICrmServiceService crmServiceService)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.crmServiceService = crmServiceService;
            this.serviceData = serviceData;

            IdsAdded = new List<int>();
        }

        public void Execute()
        {
            var result = crmServiceService.GetAll();

            unitOfWork.BeginTransaction();

            foreach (var crmService in result)
            {
                var service = unitOfWork.ServiceRepository.GetByIdCrm(crmService.Id);

                if (service == null)
                    Create(crmService);
                else
                    Update(crmService, service);
            }

            try
            {
                unitOfWork.ServiceRepository.UpdateInactives(IdsAdded);

                unitOfWork.Commit();
                serviceData.ClearKeys();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmService crmService, Domain.Models.Billing.Service service)
        {
            FillData(service, crmService);

            try
            {
                unitOfWork.ServiceRepository.Update(service);
                unitOfWork.Save();

                IdsAdded.Add(service.Id);
            }
            catch (Exception e)
            {
                logger.LogError($"Error on update Service: {crmService.Name}", e);
            }
        }

        private void Create(CrmService crmService)
        {
            var service = new Domain.Models.Billing.Service();

            FillData(service, crmService);

            try
            {
                unitOfWork.ServiceRepository.Insert(service);
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error on insert Service: {crmService.Name}", e);
            }
        }

        private void FillData(Domain.Models.Billing.Service service, CrmService crmService)
        {
            service.CrmId = crmService.Id;
            service.Name = crmService.Name;
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
