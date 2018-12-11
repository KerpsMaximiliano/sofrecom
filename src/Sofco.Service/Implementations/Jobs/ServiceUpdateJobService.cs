using System;
using System.Collections.Generic;
using AutoMapper;
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
        private readonly IMapper mapper;

        private IList<int> IdsAdded { get; }

        public ServiceUpdateJobService(IUnitOfWork unitOfWork,
            IServiceData serviceData,
            ILogMailer<ServiceUpdateJobService> logger, 
            ICrmServiceService crmServiceService, 
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.crmServiceService = crmServiceService;
            this.mapper = mapper;
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
            try
            {
                unitOfWork.ServiceRepository.Update(Translate(crmService, service));
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
            try
            {
                unitOfWork.ServiceRepository.Insert(Translate(crmService));
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error on insert Service: {crmService.Name}", e);
            }
        }

        private Domain.Models.Billing.Service Translate(CrmService crmService, Domain.Models.Billing.Service service = null)
        {
            service = service ?? new Domain.Models.Billing.Service();

            mapper.Map(crmService, service);

            service.Active = true;

            return service;
        }
    }
}
