using System;
using System.Collections.Generic;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.Managers;

namespace Sofco.Data.Billing
{
    public class ServiceData : IServiceData
    {
        private const string ServicesCacheKey = "urn:customers:{0}:services:{1}:all";
        private const string ServiceByIdCacheKey = "urn:services:id:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly ISessionManager sessionManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IRoleManager roleManager;

        public ServiceData(ICacheManager cacheManager, ISessionManager sessionManager, IUnitOfWork unitOfWork, IRoleManager roleManager)
        {
            this.cacheManager = cacheManager;
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
            this.roleManager = roleManager;
        }

        public IList<Domain.Models.Billing.Service> GetServices(string customerId, string username)
        {
            var email = sessionManager.GetUserEmail(username);

            return cacheManager.GetHashList(string.Format(ServicesCacheKey, username, customerId),
                () =>
                {
                    var hasAllAccess = roleManager.HasFullAccess();

                    if (hasAllAccess)
                        return unitOfWork.ServiceRepository.GetAllActives(customerId);
                    else
                    {
                        var user = unitOfWork.UserRepository.GetByEmail(email);
                        return unitOfWork.ServiceRepository.GetAllByManager(customerId, user.ExternalManagerId);
                    }
                },
                x => x.CrmId,
                cacheExpire);
        }

        public Domain.Models.Billing.Service GetService(Guid? serviceId)
        {
            var cacheKey = string.Format(ServiceByIdCacheKey, serviceId);

            return cacheManager.Get(cacheKey,
                () => unitOfWork.ServiceRepository.GetByIdCrm(serviceId.GetValueOrDefault().ToString()),
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(ServicesCacheKey, '*', '*'));
            cacheManager.DeletePatternKey(string.Format(ServiceByIdCacheKey, '*'));
        }
    }
}
