using System.Collections.Generic;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.TranslatorMaps
{
    public class CrmAccountTranslatorMap : ITranslatorMap
    {
        private const string Id = "accountid";
        private const string Name = "name";
        private const string Address = "address1_line1";
        private const string PostalCode = "address1_postalcode";
        private const string CityId = "_as_cityid_value";
        private const string ProvinceId = "_as_stateprovinceid_value";
        private const string Contact = "_primarycontactid_value";
        private const string CountryId = "_as_countryid_value";
        private const string Cuit = "as_governmentid";
        private const string CurrencyId = "_transactioncurrencyid_value";
        private const string OwnerId = "_ownerid_value";
        private const string PayTermCode = "paymenttermscode";
        private const string StateCode = "statecode";
        private const string Phone = "telephone1";
        private const string RelCommercial = "statuscode";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmAccount.Id), Id},
                {nameof(CrmAccount.Name), Name},
                {nameof(CrmAccount.Address), Address},
                {nameof(CrmAccount.PostalCode), PostalCode},
                {nameof(CrmAccount.City), CityId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.Contact), Contact},
                {nameof(CrmAccount.Country), CountryId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.Cuit), Cuit},
                {nameof(CrmAccount.CurrencyId), CurrencyId},
                {nameof(CrmAccount.CurrencyDescription), CurrencyId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.OwnerId), OwnerId},
                {nameof(CrmAccount.PaymentTermCode), PayTermCode},
                {nameof(CrmAccount.PaymentTermDescription), PayTermCode + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.StatusCode), StateCode},
                {nameof(CrmAccount.Telephone), Phone},
                {nameof(CrmAccount.RelCommercial), RelCommercial},
                {nameof(CrmAccount.Province), ProvinceId + TranslatorMapConstant.ODataFormattedValue}
            };
        }

        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, Address, CityId, Contact, CountryId, Cuit, CurrencyId,
                OwnerId, PayTermCode, Phone, PostalCode, StateCode, RelCommercial, ProvinceId
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
