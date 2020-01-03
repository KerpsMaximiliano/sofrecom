using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Data.Billing
{
    public class SectorData : ISectorData
    {
        private const string CacheKey = "urn:sectors:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        public SectorData(ICacheManager cacheManager, IUnitOfWork unitOfWork, IUserData userData)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public IList<Sector> GetAll()
        {
            return cacheManager.GetHashList(CacheKey,
                () => unitOfWork.UtilsRepository.GetSectors(),
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

            var delegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.PurchaseOrderApprovalOperation);

            ids.AddRange(GetAll().Where(s => s.ResponsableUserId == currentUser.Id)
                .Select(s => s.Id)
                .ToList());

            if (delegates.Any())
            {
                foreach (var userDelegate in delegates)
                {
                    ids.Add(userDelegate.AnalyticSourceId.GetValueOrDefault());
                }
            }

            return ids.Distinct().ToList();
        }
    }
}
