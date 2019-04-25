using System;
using AutoMapper;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Crm;
using Sofco.Domain.Models.Billing;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Service.Implementations.Jobs
{
    public class OpportunityUpdateJobService : IOpportunityUpdateJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<OpportunityUpdateJobService> logger;
        private readonly ICrmOpportunityService crmOpportunityService;
        private readonly IMapper mapper;

        public OpportunityUpdateJobService(IUnitOfWork unitOfWork,
            ILogMailer<OpportunityUpdateJobService> logger,
            ICrmOpportunityService crmOpportunityService, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.crmOpportunityService = crmOpportunityService;
            this.mapper = mapper;
        }

        public void Execute()
        {
            var crmOpportunities = crmOpportunityService.GetAll();

            unitOfWork.BeginTransaction();

            foreach (var crmOpportunity in crmOpportunities)
            {
                var opportunity = unitOfWork.OpportunityRepository.GetByCrmId(crmOpportunity.Id.ToString());

                if (opportunity == null)
                    Create(crmOpportunity);
                else
                    Update(crmOpportunity, opportunity);
            }

            try
            {
                unitOfWork.Commit();
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        private void Update(CrmOpportunity crmOpportunity, Opportunity opportunity)
        {
            try
            {
                unitOfWork.OpportunityRepository.Update(Translate(crmOpportunity, opportunity));
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error on update Opportunity: {crmOpportunity.Name}", e);
            }
        }

        private void Create(CrmOpportunity crmOpportunity)
        {
            try
            {
                unitOfWork.OpportunityRepository.Insert(Translate(crmOpportunity));
                unitOfWork.Save();
            }
            catch (Exception e)
            {
                logger.LogError($"Error on insert Opportunity: {crmOpportunity.Name}", e);
            }
        }

        private Opportunity Translate(CrmOpportunity data, Opportunity opportunity = null)
        {
            opportunity = opportunity ?? new Opportunity();

            var result = mapper.Map(data, opportunity);

            return result;
        }
    }
}
