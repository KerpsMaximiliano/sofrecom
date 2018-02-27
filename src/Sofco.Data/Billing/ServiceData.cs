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
    public class ServiceData : IServiceData
    {
        private const string ServicesCacheKey = "urn:customers:{0}:services:{1}:all";
        private const string ServiceByIdCacheKey = "urn:services:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ICrmHttpClient client;
        private readonly CrmConfig crmConfig;

        public ServiceData(ICacheManager cacheManager, ICrmHttpClient client, IOptions<CrmConfig> crmOptions)
        {
            this.cacheManager = cacheManager;
            this.client = client;
            crmConfig = crmOptions.Value;
        }

        public IList<CrmService> GetServices(string customerId, string identityName, string userMail, bool hasDirectorGroup)
        {
            return cacheManager.GetHashList(string.Format(ServicesCacheKey, identityName, customerId),
                () =>
                {
                    var url = hasDirectorGroup
                        ? $"{crmConfig.Url}/api/service?idAccount={customerId}"
                        : $"{crmConfig.Url}/api/service?idAccount={customerId}&idManager={userMail}";

                    var result = client.GetMany<CrmService>(url);

                    return result.Data;
                },
                x => x.Id,
                cacheExpire);
        }

        public CrmService GetService(Guid serviceId)
        {
            var cacheKey = string.Format(ServiceByIdCacheKey, serviceId);

            return cacheManager.Get(cacheKey,
                () =>
                {
                    var url = $"{crmConfig.Url}/api/service?id={serviceId}";

                    var result = client.Get<CrmService>(url);

                    return result.Data;
                },
                cacheExpire);
        }
    }
}
