﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sofco.Core.Logger;
using Sofco.Domain.Crm;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
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

        public void Update(HitoSplittedParams data, Response response)
        {
            if (string.IsNullOrWhiteSpace(data.ExternalHitoId)) return;

            try
            {
                var content = new JObject
                {
                    ["statuscode"] = data.StatusCode,
                    ["as_amount"] = data.AmmountFirstHito
                };

                httpClient.Patch<JObject>(UrlPath + "(" + data.ExternalHitoId + ")", content);
            }
            catch (Exception ex)
            {
                var msg = "CrmHitoId = " + data.ExternalHitoId;

                logger.LogError(msg, ex);

                response.AddError(msg);
            }
        }

        public void Close(Response response, string id, string status)
        {
            try
            {
                var content = new JObject
                {
                    ["statuscode"] = status,
                };

                httpClient.Patch<JObject>(UrlPath + "(" + id + ")", content);
            }
            catch (Exception ex)
            {
                var msg = "CrmHitoId = " + id;

                logger.LogError(msg, ex);

                response.AddError(msg);
            }
        }

        public void Create(HitoSplittedParams data, Response response)
        {
            var date = data.StartDate.HasValue ? data.StartDate.Value.ToString("yyyy-MM-dd") : DateTime.UtcNow.ToString("yyyy-MM-dd");

            var content = new JObject
            {
                ["as_name"] = data.Name,
                ["as_month"] = data.Month,
                ["as_date"] = date,
                ["statuscode"] = 1,
                ["as_amount"] = data.Ammount.GetValueOrDefault(),
                ["transactioncurrencyid@odata.bind"] = $"/transactioncurrencies({data.MoneyId})",
                ["as_InvoicingMilestonesId@odata.bind"] = $"/as_projects({data.ProjectId})",
                ["as_OpportunityId@odata.bind"] = $"/opportunities({data.OpportunityId})"
            };

            try
            {
                httpClient.Post<JObject>(UrlPath, content);
            }
            catch (Exception ex)
            {
                var msg = "OpportunityId: " + data.OpportunityId + " - data: " + JsonConvert.SerializeObject(content);

                logger.LogError(msg, ex);

                response.AddError(msg);
            }
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
