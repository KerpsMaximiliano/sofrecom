using System;
using System.Collections.Generic;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;

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

        public ServiceData(ICacheManager cacheManager, ISessionManager sessionManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
        }

        public IList<Model.Models.Billing.Service> GetServices(string customerId, string username)
        {
            var email = sessionManager.GetUserEmail(username);

            return cacheManager.GetHashList(string.Format(ServicesCacheKey, username, customerId),
                () =>
                {
                    var hasDirectorGroup = unitOfWork.UserRepository.HasDirectorGroup(email);
                    var hasCommercialGroup = unitOfWork.UserRepository.HasComercialGroup(email);
                    var hasAllAccess = hasDirectorGroup || hasCommercialGroup;

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

        public Model.Models.Billing.Service GetService(Guid? serviceId)
        {
            var cacheKey = string.Format(ServiceByIdCacheKey, serviceId);

            return cacheManager.Get(cacheKey,
                () => unitOfWork.ServiceRepository.GetByIdCrm(serviceId.GetValueOrDefault().ToString()),
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(ServicesCacheKey, '*'));
            cacheManager.DeletePatternKey(string.Format(ServiceByIdCacheKey, '*'));
        }
    }
}
