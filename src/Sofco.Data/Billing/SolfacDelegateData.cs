using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Enums;

namespace Sofco.Data.Billing
{
    public class SolfacDelegateData : ISolfacDelegateData
    {
        private const string SolfacDelegateCacheKey = "urn:solfacDelegates:{0}";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);
        private readonly ICacheManager cacheManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;

        public SolfacDelegateData(ICacheManager cacheManager,
            IUnitOfWork unitOfWork, IUserData userData, 
            IServiceData serviceData)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.serviceData = serviceData;
        }

        public List<string> GetUserDelegateByUserName(string userName)
        {
            var cacheKey = string.Format(SolfacDelegateCacheKey, userName);

            return cacheManager.Get(cacheKey,
                () => GetByUserName(userName),
                cacheExpire);
        }

        private List<string> GetByUserName(string userName)
        {
            var currentUserId = userData.GetByUserName(userName).Id;

            var delegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUserId, DelegationType.Solfac);

            var result = new List<string>();

            foreach (var delegation in delegates)
            {
                result.Add(delegation.User.UserName);
            }

            result.Add(userName);

            return result;
        }
    }
}
