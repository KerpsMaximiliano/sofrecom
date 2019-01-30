using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.TranslatorMaps
{
    public class CrmInvoicingMilestoneTranslatorMap : ITranslatorMap
    {
        private const string Id = "as_invoicingmilestoneid";
        private const string Name = "as_name";
        private const string Date = "as_date";
        private const string ProjectId = "_as_invoicingmilestonesid_value";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmHito.Id), Id},
                {nameof(CrmHito.Name), Name},
                {nameof(CrmHito.ScheduledDate), Date},
                {nameof(CrmHito.ProjectId), ProjectId},
                {nameof(CrmHito.ProjectName), ProjectId + TranslatorMapConstant.ODataFormattedValue}
            };
        }

        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, Date, ProjectId
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
