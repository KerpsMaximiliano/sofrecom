using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sofco.Common.Domains;
using Sofco.Core.Logger;
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

        private readonly ILogMailer<CrmServiceService> logger;

        public CrmServiceService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmService, CrmServiceTranslatorMap> translator, ILogMailer<CrmServiceService> logger)
        {
            this.httpClient = httpClient;
            this.translator = translator;
            this.logger = logger;
        }

        public List<CrmService> GetAll()
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            return translator.TranslateList(result.Data);
        }

        public Result ActivateService(Guid serviceId, bool activate = true)
        {
            var result = new Result();

            try
            {
                var body = new { statuscode = -1, statecode = activate ? 0 : 1 };

                httpClient.Patch<JObject>(UrlPath + "(" + serviceId + ")", body);
            }
            catch (Exception ex)
            {
                var msg = "CrmServiceId = " + serviceId +" - Activate = "+ activate;

                logger.LogError(msg, ex);

                result.AddError(msg);
            }

            return result;
        }

        public Result DeactivateService(Guid serviceId)
        {
            return ActivateService(serviceId, false);
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
