﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Model.Enums;

namespace Sofco.Data.Billing
{
    public class SolfacDelegateData : ISolfacDelegateData
    {
        private const string SolfacDelegateCacheKey = "urn:solfacDelegates:{0}";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);
        private readonly ICacheManager cacheManager;
        private readonly IUserDelegateRepository userDelegateRepository;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;

        public SolfacDelegateData(ICacheManager cacheManager,
            IUserDelegateRepository userDelegateRepository, IUserData userData, 
            IServiceData serviceData)
        {
            this.cacheManager = cacheManager;
            this.userDelegateRepository = userDelegateRepository;
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

            var delegates = userDelegateRepository.GetByUserId(currentUserId, UserDelegateType.Solfac);

            var result = delegates
                .Select(solfacDelegate => serviceData.GetService(solfacDelegate.ServiceId))
                .Select(service =>
                {
                    var user = userData.GetByManagerId(service.ManagerId);

                    return user.UserName;

                })
                .ToList();

            result.Add(userName);

            return result.Distinct().ToList();
        }
    }
}
