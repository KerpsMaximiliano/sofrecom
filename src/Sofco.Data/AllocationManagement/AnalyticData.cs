using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Cache;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.DAL;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Data.AllocationManagement
{
    public class AnalyticData : IAnalyticData
    {
        private const string AnalyticByIdCacheKey = "urn:analyticLites:id:{0}";

        private const string AnalyticByManagerIdCacheKey = "urn:analyticLites:managerId:{0}";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        public AnalyticData(ICacheManager cacheManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
        }

        public AnalyticLiteModel GetLiteById(int analyticId)
        {
            return cacheManager.Get(string.Format(AnalyticByIdCacheKey, analyticId),
                () => unitOfWork.AnalyticRepository.GetAnalyticLiteById(analyticId),
                cacheExpire);
        }

        public List<Analytic> GetByManagerId(int managerId)
        {
            return cacheManager.Get(string.Format(AnalyticByManagerIdCacheKey, managerId),
                () => unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(managerId).ToList(),
                cacheExpire);
        }
    }
}
