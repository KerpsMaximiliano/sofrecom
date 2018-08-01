using System;
using Sofco.Core.Cache;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Domain.Models.Admin;

namespace Sofco.Data.Admin
{
    public class SettingData : ISettingData
    {
        private const string SettingByKeyCacheKey = "urn:setting:key:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        public SettingData(ICacheManager cacheManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
        }

        public Setting GetByKey(string key)
        {
            return cacheManager.Get(string.Format(SettingByKeyCacheKey, key),
                () => unitOfWork.SettingRepository.GetByKey(key),
                cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(SettingByKeyCacheKey, '*'));
        }
    }
}
