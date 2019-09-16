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
using System;
using System.Collections.Generic;
using System.Linq;

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
            var result = httpClient.Get<JObject>(UrlPath + GetFilterByProjectQuery(projectId.ToString()));

            var data = translator.TranslateList(result.Data)
                .ToList();

            return TranslateToProjectHito(data);
        }

        private void Update(JObject content, string hitoId, Response response)
        {
            try
            {
                httpClient.Patch<JObject>(UrlPath + "(" + hitoId + ")", content);
            }
            catch (Exception ex)
            {
                var msg = "CrmHitoId = " + hitoId;

                logger.LogError(msg, ex);

                response.AddError(msg);
            }
        }

        public void UpdateAmmountAndStatus(HitoParameters data, Response response)
        {
            if (string.IsNullOrWhiteSpace(data.ExternalHitoId)) return;

            var content = new JObject
            {
                ["statuscode"] = data.StatusCode,
                ["as_amount"] = data.AmmountFirstHito
            };

            Update(content, data.ExternalHitoId, response);
        }

        public void UpdateStatus(List<string> hitoIds, HitoStatus hitoStatus)
        {
            var statusCode = (int)hitoStatus;
            var response = new Response();

            foreach (var hitoId in hitoIds)
            {
                if (string.IsNullOrWhiteSpace(hitoId)) continue;

                var content = new JObject
                {
                    ["statuscode"] = statusCode,
                };

                Update(content, hitoId, response);
            }
        }

        public void UpdateStatusAndPurchaseOrder(List<string> hitoIds, HitoStatus hitoStatus, string purchaseOrderNumber)
        {
            var statusCode = (int)hitoStatus;
            var response = new Response();

            foreach (var hitoId in hitoIds)
            {
                if (string.IsNullOrWhiteSpace(hitoId)) continue;

                var content = new JObject
                {
                    ["statuscode"] = statusCode,
                    ["as_nroordencompra"] = purchaseOrderNumber,
                };

                Update(content, hitoId, response);
            }
        }

        public void UpdateStatusAndInvoiceDateAndNumber(List<string> hitoIds, HitoStatus hitoStatus, DateTime invoicingDate, string invoiceCode)
        {
            var statusCode = (int)hitoStatus;
            var response = new Response();

            foreach (var hitoId in hitoIds)
            {
                if (string.IsNullOrWhiteSpace(hitoId)) continue;

                var content = new JObject
                {
                    ["statuscode"] = statusCode,
                    ["as_invoiceddate"] = invoicingDate.ToString("yyyy-MM-dd"),
                    ["as_invoicenumber"] = invoiceCode,
                };

                Update(content, hitoId, response);
            }
        }

        public void UpdateStatusAndBillingDate(List<string> hitoIds, HitoStatus hitoStatus, DateTime billingDate)
        {
            var statusCode = (int)hitoStatus;
            var response = new Response();

            foreach (var hitoId in hitoIds)
            {
                if (string.IsNullOrWhiteSpace(hitoId)) continue;

                var content = new JObject
                {
                    ["statuscode"] = statusCode,
                    ["as_paiddate"] = billingDate.ToString("yyyy-MM-dd"),
                };

                Update(content, hitoId, response);
            }
        }

        public void Close(Response response, string id, string status)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            var content = new JObject
            {
                ["statuscode"] = status,
            };

            Update(content, id, response);
        }

        public void UpdateAmmount(HitoAmmountParameter hito, Response response)
        {
            var content = new JObject
            {
                ["as_amount"] = hito.Ammount
            };

            Update(content, hito.Id, response);
        }

        public void Delete(string hitoId, Response response)
        {
            try
            {
                httpClient.Delete<JObject>(UrlPath + "(" + hitoId + ")", null);
            }
            catch (Exception ex)
            {
                var msg = "CrmHitoId = " + hitoId;

                logger.LogError(msg, ex);

                response.AddError(msg);
            }
        }

        public void UpdateAmmountAndName(HitoAmmountParameter hito, Response response)
        {
            var content = new JObject
            {
                ["as_name"] = hito.Name,
                ["as_month"] = hito.Month,
                ["as_amount"] = hito.Ammount
            };

            Update(content, hito.Id, response);
        }

        public CrmProjectHito GetById(string id)
        {
            var result = httpClient.Get<JObject>(UrlPath + $"({id})");

            var data = translator.Translate(result.Data);

            return mapper.Map<CrmInvoicingMilestone, CrmProjectHito>(data);
        }

        public string Create(HitoParameters data, Response response)
        {
            var date = string.Empty;

            if (data.StartDate.HasValue)
            {
#if DEBUG
                date = $"{data.StartDate.Value.Year}-{data.StartDate.Value.Month}-{data.StartDate.Value.Day}";
#else
                if(data.StartDate.Value.Day > 12){
                    date = $"{data.StartDate.Value.Year}-{data.StartDate.Value.Month}-{data.StartDate.Value.Day}";
                }
                else{
                    date = $"{data.StartDate.Value.Year}-{data.StartDate.Value.Day}-{data.StartDate.Value.Month}";
                }
#endif
            }
            else
            {
                var now = DateTime.UtcNow;
#if DEBUG
                date = $"{now.Year}-{now.Month}-{now.Day}";
#else
                date = $"{now.Year}-{now.Day}-{now.Month}";
#endif
            }

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
                var result = httpClient.Post<JObject>(UrlPath, content);

                var hito = mapper.Map<CrmInvoicingMilestone, CrmProjectHito>(translator.Translate(result.Data));

                return hito.Id;
            }
            catch (Exception ex)
            {
                var msg = "OpportunityId: " + data.OpportunityId + " - data: " + JsonConvert.SerializeObject(content);

                logger.LogError(msg, ex);

                response.AddError(msg);
            }

            return string.Empty;
        }

        private string GetQuery(string filter)
        {
            return "?$select=" + translator.KeySelects() + filter;
        }

        private string GetFilterByProjectQuery(string id)
        {
            return $"?$filter=_as_invoicingmilestonesid_value eq {id}";
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

                if (project == null) continue;

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
