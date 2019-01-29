using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
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

        private readonly ICrmTranslator<CrmHito, CrmInvoicingMilestoneTranslatorMap> translator;

        private readonly ICrmProjectService crmProjectService;

        public CrmInvoicingMilestoneService(ICrmApiHttpClient httpClient, ICrmTranslator<CrmHito, CrmInvoicingMilestoneTranslatorMap> translator, ICrmProjectService crmProjectService)
        {
            this.httpClient = httpClient;
            this.translator = translator;
            this.crmProjectService = crmProjectService;
        }

        public List<CrmHito> GetToExpire(int daysToExpire)
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            var crmHitos = Translate(result.Data)
                .Where(s => s.ScheduledDate >= DateTime.UtcNow.AddDays(daysToExpire))
                .ToList();

            return crmHitos;
        }

        private string GetQuery()
        {
            return "?$select=" + translator.KeySelects() + "&" + GetFilters();
        }

        private string GetFilters()
        {
            return $"filter=statuscode eq {HitoStatus.Projected} or statuscode eq {HitoStatus.Pending}";
        }

        private List<CrmHito> Translate(JObject data)
        {
            var projects = crmProjectService.GetAll();

            var result = translator.TranslateList(data);

            foreach (var crmHito in result)
            {
                var project = projects.FirstOrDefault(s => s.Id == crmHito.ProjectId.ToString());

                if(project == null) continue;

                crmHito.ServiceId = Guid.Parse(project.ServiceId);
                crmHito.CustomerId = Guid.Parse(project.AccountId);
                crmHito.ManagerName = project.Manager;
            }

            return result;
        }
    }
}
