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
        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmAccount, CrmAccountTranslatorMap> translator;

        private const string UrlPath = "accounts";

        public CrmAccountService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmAccount, CrmAccountTranslatorMap> translator)
        {
            this.httpClient = httpClient;
            this.translator = translator;
        }

        public List<CrmAccount> GetAll()
        {
            var result = httpClient.GetMany<CrmAccount>(UrlPath + GetQuery());

            return result.Data;
        }

        public CrmAccount GetById(Guid id)
        {
            var result = httpClient.Get<JObject>($"{UrlPath}({id})"+ GetQuery());

            var crmAccount = Translate(result.Data);

            return crmAccount;
        }

        private string GetQuery()
        {
            return "?$select=" + CrmAccountTranslatorMap.KeySelects();
        }

        private CrmAccount Translate(JObject data)
        {
            return translator.Translate(data);
        }
    }
}
