using System;
using System.Collections.Generic;
using Sofco.Core.Cache;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL.Common;
using Sofco.Model.Utils;

namespace Sofco.Data.Billing
{
    public class SectorData : ISectorData
    {
        private const string CacheKey = "urn:sectors:all";
        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;
        private readonly IUtilsRepository utilsRepository;

        public SectorData(ICacheManager cacheManager, IUtilsRepository utilsRepository)
        {
            this.cacheManager = cacheManager;
            this.utilsRepository = utilsRepository;
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
    }
}
