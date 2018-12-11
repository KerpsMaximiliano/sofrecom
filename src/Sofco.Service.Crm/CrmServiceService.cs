using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.TranslatorMaps;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmServiceService : ICrmServiceService
    {
        private const string UrlPath = "as_services";

        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmService, CrmServiceTranslatorMap> translator;

        public CrmServiceService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmService, CrmServiceTranslatorMap> translator)
        {
            this.httpClient = httpClient;
            this.translator = translator;
        }

        public List<CrmService> GetAll()
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            return translator.TranslateList(result.Data);
        }

        private string GetQuery()
        {
            return "?$select=" + translator.KeySelects() + "&" + GetFilters();
        }

        private string GetFilters()
        {
            return "$filter=statuscode eq 1";
        }
    }
}
