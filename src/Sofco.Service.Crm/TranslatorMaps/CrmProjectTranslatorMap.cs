using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.TranslatorMaps
{
    public class CrmProjectTranslatorMap : ITranslatorMap
    {
        private const string Id = "as_projectid";
        private const string Name = "as_name";
        private const string AccountId = "_as_customerid_value";
        private const string StartDate = "as_startdate";
        private const string EndDate = "as_enddate";
        private const string Analytic = "as_analtica";
        private const string Incomes = "as_estrevenue";
        private const string PurchaseOrder = "as_ordendecompra";
        private const string OpportunityId = "_as_opportunityid_value";
        private const string TotalAmount = "as_totalinvoicedamount";
        private const string ServiceType = "as_servicetype";
        private const string SolutionType = "as_solutiontype";
        private const string TechnologyType = "as_technologytype";
        private const string CurrencyId = "_transactioncurrencyid_value";
        private const string Remito = "new_remito";
        private const string ManagerId = "_as_projectmanagerid_value";
        private const string OwnerId = "_ownerid_value";
        private const string IntegratorId = "_as_integratorid_value";
        private const string ServiceId = "_as_serviceid_value";
        private const string StateCode = "statecode";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmProject.Id), Id},
                {nameof(CrmProject.Name), Name},
                {nameof(CrmProject.AccountId), AccountId},
                {nameof(CrmProject.StartDate), StartDate},
                {nameof(CrmProject.EndDate), EndDate},
                {nameof(CrmProject.Analytic), Analytic},
                {nameof(CrmProject.Incomes), Incomes},
                {nameof(CrmProject.PurchaseOrder), PurchaseOrder},
                {nameof(CrmProject.OpportunityId), OpportunityId},
                {nameof(CrmProject.TotalAmount), TotalAmount},
                {nameof(CrmProject.ServiceTypeId), ServiceType},
                {nameof(CrmProject.ServiceType), ServiceType + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmProject.SolutionTypeId), SolutionType},
                {nameof(CrmProject.SolutionType), SolutionType + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmProject.TechnologyTypeId), TechnologyType},
                {nameof(CrmProject.TechnologyType), TechnologyType + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmProject.CurrencyId), CurrencyId},
                {nameof(CrmProject.Currency), CurrencyId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmProject.Remito), Remito},
                {nameof(CrmProject.ManagerId), ManagerId},
                {nameof(CrmProject.Manager), ManagerId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmProject.OwnerId), OwnerId},
                {nameof(CrmProject.IntegratorId), IntegratorId},
                {nameof(CrmProject.Integrator), IntegratorId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmProject.ServiceId), ServiceId},
                {nameof(CrmProject.StateCode), StateCode}
            };
        }

        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, AccountId, StartDate, EndDate, Analytic, Incomes, PurchaseOrder, OpportunityId,
                TotalAmount, ServiceType, SolutionType, TechnologyType, CurrencyId, Remito, ManagerId,
                OwnerId, IntegratorId, ServiceId, StateCode
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
