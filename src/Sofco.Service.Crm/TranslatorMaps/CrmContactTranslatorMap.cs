using System.Collections.Generic;
using System.Text;
using Sofco.Domain.Crm;
using Sofco.Service.Crm.TranslatorMaps.Interfaces;

namespace Sofco.Service.Crm.TranslatorMaps
{
    public class CrmContactTranslatorMap : ITranslatorMap
    {
        private const string Id = "contactid";
        private const string Name = "fullname";
        private const string Email = "emailaddress1";
        private const string Status = "statuscode";
        private const string AccountId = "_parentcustomerid_value";

        public Dictionary<string, string> KeyMaps()
        {
            return new Dictionary<string, string>
            {
                {nameof(CrmContact.Id), Id},
                {nameof(CrmContact.Name), Name},
                {nameof(CrmContact.Email), Email},
                {nameof(CrmContact.Status), Status + TranslatorMapConstant.ODataFormattedValue},
                {nameof(CrmContact.StatusId), Status},
                {nameof(CrmContact.AccountId), AccountId},
            };
        }

        public string KeySelects()
        {
            var list = new List<string>
            {
                Name, Email, Status, AccountId
            };

            return string.Join(TranslatorMapConstant.SelectDelimiter, list);
        }
    }
}
