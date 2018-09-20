using System;
using System.Collections.Generic;
using Sofco.Core.Cache;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.Models.WorkTimeManagement;

namespace Sofco.Data.WorktimeManagement
{
    public class WorktimeData : IWorktimeData
    {
        private const string CacheKey = "urn:tigerreport:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(60);

        private readonly ICacheManager cacheManager;

        public WorktimeData(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public IList<TigerReportItem> GetAll()
        {
            return cacheManager.GetHashList(CacheKey,
                () => new List<TigerReportItem>(), 
                x => x.Id.ToString(),
                cacheExpire);
        }

        public void SaveTigerReport(IList<TigerReportItem> list)
        {
            cacheManager.SetHashList(CacheKey, list, item => item.Id.ToString(), cacheExpire);
        }

        public void ClearKeys()
        {
            cacheManager.DeletePatternKey(string.Format(CacheKey, '*'));
        }
    }
}
