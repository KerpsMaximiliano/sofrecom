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

        private const string CityId = "_as_cityid_value";

        private const string Contact = "_primarycontactid_value";

        private const string CountryId = "_as_countryid_value";

        private const string Cuit = "as_governmentid";

        private const string CurrencyId = "_transactioncurrencyid_value";

        private const string OwnerId = "_ownerid_value";

        private const string PayTermCode = "paymenttermscode";

        public static string KeySelects()
        {
            var list = new List<string>
            {
                Name, Address, CityId, Contact, CountryId, Cuit, CurrencyId, OwnerId, PayTermCode
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmAccount.Id), Id},
                {nameof(CrmAccount.Name), Name},
                {nameof(CrmAccount.Address), Address},
                {nameof(CrmAccount.City), CityId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.Contact), Contact},
                {nameof(CrmAccount.Country), CountryId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.Cuit), Cuit},
                {nameof(CrmAccount.CurrencyDescription), CurrencyId + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.OwnerId), OwnerId},
                {nameof(CrmAccount.PaymentTermCode), PayTermCode}
            };
        }
    }
}
