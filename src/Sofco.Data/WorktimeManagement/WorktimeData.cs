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
        private const string CacheControlHoursKey = "urn:{0}:controlhours:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(60);

        private readonly ICacheManager cacheManager;

        public WorktimeData(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public IList<TigerReportItem> GetAllTigerReport()
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

        public void ClearTigerReportKey()
        {
            cacheManager.DeletePatternKey(string.Format(CacheKey, '*'));
        }

        public IList<WorkTimeControlResourceModel> GetAllControlHoursReport(string username)
        {
            var key = string.Format(CacheControlHoursKey, username);

            return cacheManager.GetHashList(key,
                () => new List<WorkTimeControlResourceModel>(),
                x => x.Id.ToString(),
                cacheExpire);
        }

        public void SaveControlHoursReport(IList<WorkTimeControlResourceModel> list, string username)
        {
            var key = string.Format(CacheControlHoursKey, username);

            cacheManager.SetHashList(key, list, item => item.Id.ToString(), cacheExpire);
        }

        public void ClearControlHoursReportKey(string username)
        {
            var key = string.Format(CacheControlHoursKey, username);

            cacheManager.DeletePatternKey(key);
        }
    }
}
