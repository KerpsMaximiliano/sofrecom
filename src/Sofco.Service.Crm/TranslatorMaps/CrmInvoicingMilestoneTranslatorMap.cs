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
        private const string MoneyId = "_transactioncurrencyid_value";
        private const string Month = "as_month";
        private const string StatusCode = "statuscode";
        private const string OpportunityId = "_as_opportunityid_value";
        private const string PurchaseOrder = "as_nroordencompra";
        private const string Amount = "as_amount";
        private const string BaseAmount = "as_amount_base";
        private const string AmountOriginal = "as_montoestimado";
        private const string BaseAmountOriginal = "as_montoestimado_base";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmInvoicingMilestone.Id), Id},
                {nameof(CrmInvoicingMilestone.Name), Name},
                 {nameof(CrmInvoicingMilestone.Date), Date + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmInvoicingMilestone.ProjectId), ProjectId},
                {nameof(CrmInvoicingMilestone.ProjectName), ProjectId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmInvoicingMilestone.MoneyId), MoneyId},
                {nameof(CrmInvoicingMilestone.Money), MoneyId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmInvoicingMilestone.Amount), Amount},
                {nameof(CrmInvoicingMilestone.Month), Month},
                {nameof(CrmInvoicingMilestone.StatusCode), StatusCode},
                {nameof(CrmInvoicingMilestone.Status), StatusCode + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmInvoicingMilestone.OpportunityId), OpportunityId},
                {nameof(CrmInvoicingMilestone.OpportunityName), OpportunityId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmInvoicingMilestone.PurchaseOrder), PurchaseOrder},
                {nameof(CrmInvoicingMilestone.BaseAmount), BaseAmount},
                {nameof(CrmInvoicingMilestone.AmountOriginal), AmountOriginal},
                {nameof(CrmInvoicingMilestone.BaseAmountOriginal), BaseAmountOriginal},
            };
        }
         
        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, Date, ProjectId, MoneyId, Amount, Month, StatusCode,
                OpportunityId, PurchaseOrder, BaseAmount, AmountOriginal, BaseAmountOriginal
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
