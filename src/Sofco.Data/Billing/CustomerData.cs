using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Cache;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.Service.Http.Interfaces;

namespace Sofco.Data.Billing
{
    public class CustomerData : ICustomerData
    {
        private const string CustomersCacheKey = "urn:customers:{0}:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;

        public CustomerData(ICacheManager cacheManager, ICrmHttpClient client, IOptions<CrmConfig> crmOptions)
        {
            this.cacheManager = cacheManager;
            this.client = client;
            this.crmConfig = crmOptions.Value;
        }

        public IList<CrmCustomer> GetCustomers(string identityName, string userMail, bool hasDirectorGroup)
        {
            return cacheManager.GetHashList(string.Format(CustomersCacheKey, identityName),
                () =>
                {
                    var url = hasDirectorGroup
                        ? $"{crmConfig.Url}/api/account"
                        : $"{crmConfig.Url}/api/account?idManager={userMail}";

                    var result = client.GetMany<CrmCustomer>(url);

                    return result.Data;
                },
                x => x.Id,
                cacheExpire);
        }
    }
}
