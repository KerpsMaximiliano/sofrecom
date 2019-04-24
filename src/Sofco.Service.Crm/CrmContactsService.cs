using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.TranslatorMaps;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmContactService : ICrmContactService
    {
        private const string UrlPath = "contacts";

        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmContact, CrmContactTranslatorMap> translator;

        public CrmContactService(ICrmApiHttpClient httpClient,
            ICrmTranslator<CrmContact, CrmContactTranslatorMap> translator)
        {
            this.httpClient = httpClient;
            this.translator = translator;
        }

        public List<CrmContact> GetAll()
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
