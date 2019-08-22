using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sofco.Common.Domains;
using Sofco.Core.Logger;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.Settings;
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

        private readonly CrmSetting crmSetting;

        public CrmServiceService(ICrmApiHttpClient httpClient,
            ICrmTranslator<CrmService, CrmServiceTranslatorMap> translator,
            ILogMailer<CrmServiceService> logger, IOptions<CrmSetting> crmOptions)
        {
            this.httpClient = httpClient;
            this.translator = translator;
            this.logger = logger;
            crmSetting = crmOptions.Value;
        }

        public List<CrmService> GetAll()
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            return translator.TranslateList(result.Data);
        }

        public Result ActivateService(Guid serviceId, bool activate = true)
        {
            var result = new Result();

            if (serviceId == Guid.Empty)
            {
                return result;
            }

            try
            {
                var body = new { statuscode = -1, statecode = activate ? 0 : 1 };

                httpClient.Patch<JObject>(UrlPath + "(" + serviceId + ")", body);
            }
            catch (Exception ex)
            {
                var msg = "CrmServiceId = " + serviceId + " - Activate = " + activate;

                logger.LogError(msg, ex);

                result.AddError(msg);
            }

            return result;
        }

        public Result DeactivateService(Guid serviceId)
        {
            return ActivateService(serviceId, false);
        }

        public Result Update(CrmServiceUpdate crmServiceUpdate)
        {
            var result = new Result();

            if (crmServiceUpdate.Id == Guid.Empty)
            {
                return result;
            }
             
            var serviceId = crmServiceUpdate.Id;

            var content = new JObject
            {
                ["as_analitica"] = crmServiceUpdate.AnalyticTitle,
                ["as_startdate_date"] = crmServiceUpdate.StartDate,
                ["as_enddate_date"] = crmServiceUpdate.EndDate,
                ["as_descriptionspanish"] = crmServiceUpdate.Description,
            };

            if (crmServiceUpdate.ManagerId.HasValue)
                content["as_projectmanagerid@odata.bind"] = GetCrmUserUrl(crmServiceUpdate.ManagerId.Value);

            if (crmServiceUpdate.ServiceTypeId.HasValue)
                content["as_servicetype"] = crmServiceUpdate.ServiceTypeId.Value;

            if (crmServiceUpdate.SoluctionTypeId.HasValue)
                content["as_solutiontype"] = crmServiceUpdate.SoluctionTypeId.Value;

            if (crmServiceUpdate.TechnologyTypeId.HasValue)
                content["as_technologytype"] = crmServiceUpdate.TechnologyTypeId.Value;

            try
            {
                httpClient.Patch<JObject>(UrlPath + "(" + serviceId + ")", content);
            }
            catch (Exception ex)
            {
                var msg = "CrmServiceId: " + serviceId + " - data: " + JsonConvert.SerializeObject(content);

                logger.LogError(msg, ex);

                result.AddError(msg);
            }

            return result;
        }

        private string GetQuery()
        {
            return "?$select=" + translator.KeySelects();
        }

        private string GetCrmUserUrl(Guid crmUserId)
        {
            return crmSetting.UrlApi + "systemusers(" + crmUserId + ")";
        }
    }
}
