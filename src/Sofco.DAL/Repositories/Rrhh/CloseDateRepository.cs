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

        public Tuple<CloseDate, CloseDate> GetBeforeAndCurrent(int id)
        {
            var closeDate = context.CloseDates.SingleOrDefault(x => x.Id == id);
            var closeDateBefore = new CloseDate();
            var closeMonthSetting = context.Settings.SingleOrDefault(x => x.Key.Equals(SettingConstant.CloseMonthKey));

            if (closeDate != null)
            {
                var dateBefore = new DateTime(closeDate.Year, closeDate.Month, 1).AddMonths(-1);

                var closeDateBeforeAux = context.CloseDates.SingleOrDefault(x => x.Year == dateBefore.Year && x.Month == dateBefore.Month);

                if (closeDateBeforeAux != null)
                {
                    closeDateBefore = closeDateBeforeAux;
                }
                else
                {
                    closeDateBefore.Year = dateBefore.Year;
                    closeDateBefore.Month = dateBefore.Month;
                    closeDateBefore.Day = closeMonthSetting != null ? Convert.ToInt32(closeMonthSetting.Value) : 20;
                }
            }

            return new Tuple<CloseDate, CloseDate>(closeDate, closeDateBefore);
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

        public IList<CloseDate> GetAllBeforeNextMonth()
        {
            var today = DateTime.UtcNow.Date.AddMonths(1);

            return context.CloseDates
                .Where(x => new DateTime(x.Year, x.Month, 1).Date <= today)
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .Take(5)
                .ToList();
        }
        public CloseDate GetFirstBeforeNextMonth()
        {
            var today = DateTime.UtcNow.Date;

            return context.CloseDates.Where(x => new DateTime(x.Year, x.Month, x.Day).Date <= today)
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ThenByDescending(x => x.Day)
                .Take(1)
                .FirstOrDefault();
        }
    }
}
