using System;

namespace Sofco.Core.Models.Common
{
    public class CloseDatesSettings : Tuple<int, int, int>
    {
        public CloseDatesSettings(int current, int before, int next) : base(current, before, next)
        {
        }

        public int CurrentCloseMonthDay => Item1;

        public int BeforeCloseMonthDay => Item2;

        public int NextCloseMonthDay => Item3;

        public Tuple<DateTime, DateTime> GetPeriodExcludeDays()
        {
            var now = DateTime.Now.Date;

            DateTime dateFrom;
            DateTime dateTo;

            if (now.Day > CurrentCloseMonthDay)
            {
                dateFrom = new DateTime(now.Year, now.Month, 1);
                dateTo = new DateTime(now.Year, now.Month, 1).AddMonths(1);
            }
            else
            {
                dateFrom = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                dateTo = new DateTime(now.Year, now.Month, 1);
            }

            return new Tuple<DateTime, DateTime>(dateFrom, dateTo);
        }

        public Tuple<DateTime, DateTime> GetPeriodIncludeDays()
        {
            var now = DateTime.Now.Date;

            DateTime dateFrom;
            DateTime dateTo;

            if (now.Day > CurrentCloseMonthDay)
            {
                dateFrom = new DateTime(now.Year, now.Month, CurrentCloseMonthDay).AddDays(1);
                dateTo = new DateTime(now.Year, now.Month, NextCloseMonthDay).AddMonths(1);
            }
            else
            {
                dateFrom = new DateTime(now.Year, now.Month, BeforeCloseMonthDay).AddMonths(-1).AddDays(1);
                dateTo = new DateTime(now.Year, now.Month, CurrentCloseMonthDay);
            }

            return new Tuple<DateTime, DateTime>(dateFrom, dateTo);
        }
    }
}