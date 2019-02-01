using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Sofco.Common.Domains;
using Sofco.Core.Logger;
using Sofco.Domain.Crm;
using Sofco.Domain.Enums;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.TranslatorMaps;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmInvoicingMilestoneService : ICrmInvoicingMilestoneService
    {
        private const string UrlPath = "as_invoicingmilestones";

        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmInvoicingMilestone, CrmInvoicingMilestoneTranslatorMap> translator;

        private readonly ICrmProjectService crmProjectService;

        private readonly IMapper mapper;

        private readonly ILogMailer<CrmInvoicingMilestoneService> logger;

        public CrmInvoicingMilestoneService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmInvoicingMilestone, CrmInvoicingMilestoneTranslatorMap> translator, 
            ICrmProjectService crmProjectService, IMapper mapper, 
            ILogMailer<CrmInvoicingMilestoneService> logger)
        {
            this.httpClient = httpClient;
            this.translator = translator;
            this.crmProjectService = crmProjectService;
            this.mapper = mapper;
            this.logger = logger;
        }

        public List<CrmHito> GetToExpire(int daysToExpire)
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery(GetToExpireFilters()));

            var data = translator.TranslateList(result.Data)
                .Where(s => s.Date >= DateTime.UtcNow.AddDays(daysToExpire))
                .ToList();

            return TranslateToExpireHito(data);
        }

        public List<CrmProjectHito> GetByProjectId(Guid projectId)
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery(string.Empty));

            var data = translator.TranslateList(result.Data)
                .Where(s => s.ProjectId == projectId)
                .ToList();

            return TranslateToProjectHito(data);
        }

        public Result Update(CrmInvoicingMilestone data)
        {
            var result = new Result();

            if (data.Id == Guid.Empty)
            {
                return result;
            }

            try
            {
            var body = new { statuscode = 0 };

            httpClient.Patch<JObject>(UrlPath + "(" + data.Id + ")", body);
            }
            catch (Exception ex)
            {
                var msg = "CrmHitoId = " + data.Id;

                logger.LogError(msg, ex);

                result.AddError(msg);
            }

            return result;
        }

        public Result Create(CrmInvoicingMilestone data)
        {
            throw new NotImplementedException();
        }

        private string GetQuery(string filter)
        {
            return "?$select=" + translator.KeySelects() + filter;
        }

        private string GetToExpireFilters()
        {
            return $"filter=statuscode eq {HitoStatus.Projected} or statuscode eq {HitoStatus.Pending}";
        }

        private List<CrmHito> TranslateToExpireHito(List<CrmInvoicingMilestone> data)
        {
            var projects = crmProjectService.GetAll();

            var crmHitos = mapper.Map<List<CrmInvoicingMilestone>, List<CrmHito>>(data);

            foreach (var crmHito in crmHitos)
            {
                var project = projects.FirstOrDefault(s => s.Id == crmHito.ProjectId.ToString());

                if(project == null) continue;

                crmHito.ServiceId = Guid.Parse(project.ServiceId);
                crmHito.CustomerId = Guid.Parse(project.AccountId);
                crmHito.ManagerName = project.Manager;
            }

            return crmHitos;
        }

        private List<CrmProjectHito> TranslateToProjectHito(List<CrmInvoicingMilestone> data)
        {
            return mapper.Map<List<CrmInvoicingMilestone>, List<CrmProjectHito>>(data);
        }
    }
}
