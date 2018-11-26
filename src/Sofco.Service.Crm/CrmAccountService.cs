using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.TranslatorMaps;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmAccountService : ICrmAccountService
    {
        private const string UrlPath = "accounts";

        private const int IntegratorClientStatusCode = 717620000;

        private const int DirectClientStatusCode = 717620001;

        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmAccount, CrmAccountTranslatorMap> translator;

        public CrmAccountService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmAccount, CrmAccountTranslatorMap> translator)
        {
            this.httpClient = httpClient;
            this.translator = translator;
        }

        public List<CrmAccount> GetAll()
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            return translator.TranslateList(result.Data);
        }

        public CrmAccount GetById(Guid id)
        {
            var result = httpClient.Get<JObject>($"{UrlPath}({id})"+ GetQuery());

            return translator.Translate(result.Data);
        }

        private string GetQuery()
        {
            return "?$select=" + translator.KeySelects() +"&"+ GetFilters();
        }

        private string GetFilters()
        {
            return $"$filter=statuscode eq {IntegratorClientStatusCode} or statuscode eq {DirectClientStatusCode}";
        }
    }
}
