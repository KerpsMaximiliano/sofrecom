using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.TranslatorMaps
{
    public class CrmOpportunityTranslatorMap : ITranslatorMap
    {
        private const string Id = "opportunityid";
        private const string Name = "name";
        private const string Number = "as_opportunitynumber";
        private const string ActualValue = "actualvalue";
        private const string ParentContactId = "_parentcontactid_value";
        private const string ProjectManagerId = "_as_gerentedeproyecto_value";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmOpportunity.Id), Id},
                {nameof(CrmOpportunity.Name), Name},
                {nameof(CrmOpportunity.Number), Number},
                {nameof(CrmOpportunity.ActualValue), Number},
                {nameof(CrmOpportunity.ProjectManagerId), ProjectManagerId},
                {nameof(CrmOpportunity.ProjectManagerName), ProjectManagerId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmOpportunity.ParentContactId), ParentContactId},
                {nameof(CrmOpportunity.ParentContactName), ParentContactId + TranslatorMapConstant.ODataFormattedValue},
            };
        }

        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, Number, ActualValue, ParentContactId, ProjectManagerId
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
