using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.HttpClients.Interfaces;
using Sofco.Service.Crm.Interfaces;
using Sofco.Service.Crm.TranslatorMaps;
using Sofco.Service.Crm.Translators.Interfaces;

namespace Sofco.Service.Crm
{
    public class CrmProjectService : ICrmProjectService
    {
        private const string UrlPath = "as_projects";

        private readonly ICrmApiHttpClient httpClient;

        private readonly ICrmTranslator<CrmProject, CrmProjectTranslatorMap> translator;

        private readonly ICrmOpportunityService crmOpportunityService;

        public CrmProjectService(ICrmApiHttpClient httpClient, 
            ICrmTranslator<CrmProject, CrmProjectTranslatorMap> translator, ICrmOpportunityService crmOpportunityService)
        {
            this.httpClient = httpClient;
            this.translator = translator;
            this.crmOpportunityService = crmOpportunityService;
        }

        public List<CrmProject> GetAll()
        {
            var result = httpClient.Get<JObject>(UrlPath + GetQuery());

            return Translate(result.Data);
        }

        private string GetQuery()
        {
            return "?$select=" + translator.KeySelects();
        }

        private List<CrmProject> Translate(JObject data)
        {
            var opportunities = crmOpportunityService.GetAll();

            var result = translator.TranslateList(data);

            foreach (var crmProject in result)
            {
                var opportunity = opportunities.FirstOrDefault(s => s.Id.ToString() == crmProject.OpportunityId);

                if(opportunity == null) continue;

                crmProject.OpportunityName = opportunity.Name;
                crmProject.OpportunityNumber = opportunity.Number;
                if (opportunity.ActualValue != null) crmProject.RealIncomes = opportunity.ActualValue.Value;
                crmProject.PrincipalContactId = opportunity.ParentContactId;
                crmProject.PrincipalContactName = opportunity.ParentContactName;
            }

            return result;
        }
    }
}
