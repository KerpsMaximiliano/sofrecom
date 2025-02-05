﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Domain.Models.WorkTimeManagement;
using Spire.Pdf.Exporting.XPS.Schema;

namespace Sofco.Framework.StatusHandlers.License
{
    public class LicenseValidator
    {
        protected Tuple<int, int> GetNumberOfWorkingDays(DateTime start, DateTime stop, IUnitOfWork unitOfWork, List<Holiday> holidays)
        {
            //var holidays = unitOfWork.HolidayRepository.Get(start.Year, start.Month);

            if (start.Month != stop.Month)
                holidays.AddRange(unitOfWork.HolidayRepository.Get(stop.Year, stop.Month));

            int workingDays = 0;
            int totalDays = 0;

            while (start.Date <= stop.Date)
            {
                if (start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday && holidays.All(x => x.Date.Date != start.Date))
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
