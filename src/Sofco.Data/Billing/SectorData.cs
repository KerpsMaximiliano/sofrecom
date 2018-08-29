using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Domain.Utils;

namespace Sofco.Data.Billing
{
    public class SectorData : ISectorData
    {
        private const string CacheKey = "urn:sectors:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUtilsRepository utilsRepository;

        private readonly IUserData userData;

        public SectorData(ICacheManager cacheManager, IUtilsRepository utilsRepository, IUserData userData)
        {
            this.cacheManager = cacheManager;
            this.utilsRepository = utilsRepository;
            this.userData = userData;
        }

        public IList<Sector> GetAll()
        {
            return cacheManager.GetHashList(CacheKey,
                () => utilsRepository.GetSectors(),
                x => x.Id.ToString(),
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(CacheKey, '*'));
        }

        public List<int> GetIdByCurrent()
        {
            var currentUser = userData.GetCurrentUser();

            return GetAll().Where(s => s.ResponsableUserId == currentUser.Id)
                .Select(s => s.Id)
                .ToList();
        }
    }
}
