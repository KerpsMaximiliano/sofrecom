using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.TranslatorMaps
{
    public class CrmServiceTranslatorMap : ITranslatorMap
    {
        private const string Id = "as_serviceid";
        private const string Name = "as_name";
        private const string AccountId = "_as_customerid_value";
        private const string IndustryId = "_as_industryid_value";
        private const string StartDate = "as_startdate";
        private const string EndDate = "as_enddate";
        private const string ManagerId = "_as_projectmanagerid_value";
        private const string ServiceTypeId = "as_servicetype";
        private const string SolutionTypeId = "as_solutiontype";
        private const string TechnologyTypeId = "as_technologytype";
        private const string Analytic = "as_analitica";
        private const string StateCode = "statecode";
        private const string Description = "as_descriptionspanish";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmService.Id), Id},
                {nameof(CrmService.Name), Name},
                {nameof(CrmService.AccountId), AccountId},
                {nameof(CrmService.AccountName), AccountId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.Industry), IndustryId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.StartDate), StartDate + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.EndDate), EndDate + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.ManagerId), ManagerId},
                {nameof(CrmService.Manager), ManagerId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.ServiceTypeId), ServiceTypeId},
                {nameof(CrmService.ServiceType), ServiceTypeId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.SolutionTypeId), SolutionTypeId},
                {nameof(CrmService.SolutionType), SolutionTypeId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.TechnologyTypeId), TechnologyTypeId},
                {nameof(CrmService.TechnologyType), TechnologyTypeId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmService.Analytic), Analytic},
                {nameof(CrmService.StateCode), StateCode},
                {nameof(CrmService.Description), Description},
            };
        }

        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, AccountId, IndustryId, StartDate, EndDate, ManagerId, ServiceTypeId,
                SolutionTypeId, TechnologyTypeId, Analytic, StateCode, Description
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
