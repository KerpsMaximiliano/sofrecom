using System.Collections.Generic;
using Sofco.Domain.Crm;

namespace Sofco.Service.Crm.ModelMaps
{
    public class CrmAccountModelMap
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

        public static string GetSelects()
        {
            var list = new List<string>
            {
                Name, Address, CityId, Contact, CountryId, Cuit, CurrencyId, OwnerId, PayTermCode
            };

            return string.Join(",", list);
        }

        public static Dictionary<string, string> GetKeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmAccount.Id), Id},
                {nameof(CrmAccount.Name), Name},
                {nameof(CrmAccount.Address), Address},
                {nameof(CrmAccount.City), CityId + ModelMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.Contact), Contact},
                {nameof(CrmAccount.Country), CountryId + ModelMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.Cuit), Cuit},
                {nameof(CrmAccount.CurrencyDescription), CurrencyId + ModelMapConstant.ODataFormattedValue},
                {nameof(CrmAccount.OwnerId), OwnerId},
                {nameof(CrmAccount.PaymentTermCode), PayTermCode}
            };
        }
    }
}
