using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm;
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
            return "?$select=" + translator.KeySelects();
        }
    }
}
