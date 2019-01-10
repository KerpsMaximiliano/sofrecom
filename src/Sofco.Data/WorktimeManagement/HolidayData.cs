using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Core.Cache;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.DAL;
using Sofco.Domain.Models.WorkTimeManagement;

namespace Sofco.Data.WorktimeManagement
{
    public class HolidayData : IHolidayData
    {
        private const string CacheKey = "urn:holidays:all";

        private readonly TimeSpan cacheExpire = TimeSpan.FromMinutes(10);

        private readonly ICacheManager cacheManager;

        private readonly IUnitOfWork unitOfWork;

        public HolidayData(ICacheManager cacheManager, IUnitOfWork unitOfWork)
        {
            this.cacheManager = cacheManager;
            this.unitOfWork = unitOfWork;
        }

        public List<Holiday> Get(int year, int month)
        {
            var result = cacheManager.GetHashList(CacheKey,
                () => unitOfWork.HolidayRepository.GetAll(),
                x => x.Id.ToString(),
                cacheExpire);

            return result
                .Where(s => s.Date.ToUniversalTime().Year == year
                            && s.Date.ToUniversalTime().Month == month)
                .OrderBy(s => s.Date)
                .ToList();
        }
    }
}
