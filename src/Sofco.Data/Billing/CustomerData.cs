using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Config;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL.Admin;
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
        private readonly IUserRepository userRepository;
        private readonly ISessionManager sessionManager;

        public CustomerData(ICacheManager cacheManager, ICrmHttpClient client, IOptions<CrmConfig> crmOptions, IUserRepository userRepository, ISessionManager sessionManager)
        {
            this.cacheManager = cacheManager;
            this.client = client;
            this.userRepository = userRepository;
            this.sessionManager = sessionManager;
            crmConfig = crmOptions.Value;
        }

        public IList<CrmCustomer> GetCustomers(string userMail)
        {
            var email = userMail.IndexOf('@') > -1 ? userMail : $"{userMail}@sofrecom.com.ar";

            var identityName = sessionManager.GetUserName();

            return cacheManager.GetHashList(string.Format(CustomersCacheKey, identityName),
                () =>
                {
                    var hasDirectorGroup = userRepository.HasDirectorGroup(email);
                    var hasCommercialGroup = userRepository.HasComercialGroup(email);
                    var hasAllAccess = hasDirectorGroup || hasCommercialGroup;

                    var url = hasAllAccess
                        ? $"{crmConfig.Url}/api/account"
                        : $"{crmConfig.Url}/api/account?idManager={email}";

                    var result = client.GetMany<CrmCustomer>(url);

                    return result.Data;
                },
                x => x.Id,
                cacheExpire);
        }
    }
}
