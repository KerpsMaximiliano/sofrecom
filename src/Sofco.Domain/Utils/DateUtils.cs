using System;

namespace Sofco.Domain.Utils
{
    public class DateUtils
    {
        public static DateTime GetTimeNowArgentina()
        {
            TimeZoneInfo objTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            DateTimeOffset dt = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, objTimeZoneInfo);
            return dt.DateTime;
        }
    }
}
