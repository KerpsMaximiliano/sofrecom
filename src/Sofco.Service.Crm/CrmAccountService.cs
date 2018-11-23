using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.ModelMaps;
using Sofco.Service.Crm.Translators;

namespace Sofco.Service.Crm
{
    public class CrmAccountService : ICrmAccountService
    {
        private readonly ICrmApiHttpClient httpClient;

        public CrmAccountService(ICrmApiHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public List<CrmAccount> GetAll()
        {
            var result = httpClient.GetMany<CrmAccount>("accounts");

            return result.Data;
        }

        public CrmAccount GetById(Guid id)
        {
            var result = httpClient.Get<JObject>($"accounts({id})?$select="+ CrmAccountModelMap.GetSelects());

            var crmAccount = Translate(result.Data);

            return crmAccount;
        }

        private CrmAccount Translate(JObject data)
        {
            var translator = new CrmTranslator<CrmAccount>();

            var crmAccount = translator.Translate(data, CrmAccountModelMap.GetKeyMaps());

            return crmAccount;
        }
    }
}
