﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Data.Billing
{
    public class AreaData : IAreaData
    {
        private const string CacheKey = "urn:areas:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        public AreaData(ICacheManager cacheManager, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public IList<Area> GetAll()
        {
            return cacheManager.GetHashList(CacheKey,
                () => unitOfWork.UtilsRepository.GetAreas(),
                x => x.Id.ToString(),
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(CacheKey, '*'));
        }

        public List<int> GetIdByCurrent()
        {
            var ids = new List<int>();

            var currentUser = userData.GetCurrentUser();

            var delegates = unitOfWork.UserDelegateRepository.GetByUserIdAndType(currentUser.Id, UserDelegateType.PurchaseOrderApprovalCommercial);

            if (delegates.Any())
            {
                foreach (var userDelegate in delegates)
                {
                    ids.AddRange(GetAll().Where(s => s.ResponsableUserId == userDelegate.SourceId.GetValueOrDefault())
                        .Select(s => s.Id)
                        .ToList());
                }
            }

            ids.AddRange(GetAll().Where(s => s.ResponsableUserId == currentUser.Id)
                .Select(s => s.Id)
                .ToList());

            return ids.Distinct().ToList();
        }
    }
}
