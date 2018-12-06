using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.TranslatorMaps;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmOpportunityService : ICrmOpportunityService
    {
        private const string UrlPath = "opportunities";

        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmOpportunity, CrmOpportunityTranslatorMap> translator;

        public CrmOpportunityService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmOpportunity, CrmOpportunityTranslatorMap> translator)
        {
            this.httpClient = httpClient;
            this.translator = translator;
        }

        public List<CrmOpportunity> GetAll()
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            return translator.TranslateList(result.Data);
        }

        private string GetQuery()
        {
            return "?$select=" + translator.KeySelects(); // + "&$filter=opportunityid eq 62dc7b03-b5d7-e811-a971-000d3ac1b8d2";
        }
    }
}
