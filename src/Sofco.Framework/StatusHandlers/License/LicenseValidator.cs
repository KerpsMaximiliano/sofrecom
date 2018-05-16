using System;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseValidator
    {
        protected Tuple<int, int> GetNumberOfWorkingDays(DateTime start, DateTime stop)
        {
            int workingDays = 0;
            int totalDays = 0;

            while (start.Date <= stop.Date)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }

                totalDays++;

                start = start.AddDays(1);
            }

            return new Tuple<int, int>(workingDays, totalDays);
        }
    }
}
