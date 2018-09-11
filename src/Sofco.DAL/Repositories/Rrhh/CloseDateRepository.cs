using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Rrhh;
using Sofco.Core.Models.Common;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.DAL.Repositories.Rrhh
{
    public class CloseDateRepository : BaseRepository<CloseDate>, ICloseDateRepository
    {
        public CloseDateRepository(SofcoContext context) : base(context)
        {
        }

        public IList<CloseDate> Get(int startMonth, int startYear, int endMonth, int endYear)
        {
            return context.CloseDates.Where(x =>
                    new DateTime(x.Year, x.Month, 1).Date >= new DateTime(startYear, startMonth, 1).Date &&
                    new DateTime(x.Year, x.Month, 1).Date <= new DateTime(endYear, endMonth, 1).Date)
                .ToList();
        }
         
        public CloseDatesSettings GetBeforeCurrentAndNext()
        {
            var now = DateTime.UtcNow.Date;
            var today = new DateTime(now.Year, now.Month, 1);
            var before = now.AddMonths(-1);
            var next = now.AddMonths(1);

            var dates = context.CloseDates.Where(x =>
                    new DateTime(x.Year, x.Month, 1).Date >= before &&
                    new DateTime(x.Year, x.Month, 1).Date <= next)
                .ToList();

            var closeMonthSetting = context.Settings.SingleOrDefault(x => x.Key.Equals(SettingConstant.CloseMonthKey));

            var currentDay = 0;
            var beforeDay = 0;
            var nextDay = 0;

            if (closeMonthSetting != null)
            {
                currentDay = Convert.ToInt32(closeMonthSetting.Value);
                beforeDay = Convert.ToInt32(closeMonthSetting.Value);
                nextDay = Convert.ToInt32(closeMonthSetting.Value);
            }

            foreach (var closeDate in dates)
            {
                if (closeDate.Year == today.Year && closeDate.Month == today.Month && closeDate.Day > 0)
                    currentDay = closeDate.Day;

                if (closeDate.Year == before.Year && closeDate.Month == before.Month && closeDate.Day > 0)
                    beforeDay = closeDate.Day;

                if (closeDate.Year == next.Year && closeDate.Month == next.Month && closeDate.Day > 0)
                    nextDay = closeDate.Day;
            }

            var result = new CloseDatesSettings(currentDay, beforeDay, nextDay);

            return result;
        }
    }
}
