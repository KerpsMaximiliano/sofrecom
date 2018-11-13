using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain;
using Sofco.Domain.Enums;

namespace Sofco.Framework.Managers
{
    public class WorkTimeResumeManager : IWorkTimeResumeManager
    {
        private const int DefaultMaxHourPerDay = 8;

        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        public WorkTimeResumeManager(IUnitOfWork unitOfWork, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
        }

        public WorkTimeResumeModel GetResume(List<WorkTimeCalendarModel> calendarModels, DateTime startDateTime, DateTime endDateTime)
        {
            var result = new WorkTimeResumeModel
            {
                HoursApproved = calendarModels.Where(x => x.Status == WorkTimeStatus.Approved).Sum(x => x.Hours),
                HoursRejected = calendarModels.Where(x => x.Status == WorkTimeStatus.Rejected).Sum(x => x.Hours),
                HoursPending = calendarModels.Where(x => x.Status == WorkTimeStatus.Draft).Sum(x => x.Hours),
                HoursPendingApproved = calendarModels.Where(x => x.Status == WorkTimeStatus.Sent).Sum(x => x.Hours),
                HoursWithLicense = calendarModels.Where(x => x.Status == WorkTimeStatus.License).Sum(x => x.Hours)
            };

            var maxHourPerDay = GetMaxHourPerDay();

            var startDate = startDateTime.Date;

            var endDate = endDateTime.Date;

            var indexDate = startDate;

            while (indexDate <= endDate)
            {
                if (indexDate.DayOfWeek != DayOfWeek.Saturday && indexDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    result.BusinessHours += maxHourPerDay;

                    if (indexDate <= DateTime.UtcNow.Date)
                    {
                        result.HoursUntilToday += maxHourPerDay;
                    }
                }

                indexDate = indexDate.AddDays(1);
            }

            var holidays = unitOfWork.HolidayRepository.Get(startDate.Year, startDate.Month);

            holidays.AddRange(unitOfWork.HolidayRepository.Get(endDate.Year, endDate.Month));

            holidays = holidays.Where(x => x.Date >= startDate && x.Date <= endDate).ToList();

            if (holidays.Any())
            {
                result.BusinessHours -= holidays.Count * maxHourPerDay;
            }

            return result;
        }

        public WorkTimeResumeModel GetCurrentPeriodResume()
        {
            var currentUserId = userData.GetCurrentUser().Id;

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            var period = closeDates.GetPeriodIncludeDays();

            var startDate = period.Item1;

            var endDate = period.Item2;

            var workTimes = unitOfWork.WorkTimeRepository.Get(startDate, endDate, currentUserId);

            var calendars = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

            return GetResume(calendars, startDate, endDate);
        }

        private int GetMaxHourPerDay()
        {
            var hourSetting = unitOfWork.SettingRepository.GetByKey(SettingConstant.WorkingHoursPerDaysMaxKey);

            if (hourSetting == null) return DefaultMaxHourPerDay;

            var result = int.Parse(hourSetting.Value);

            return result == 0 ? DefaultMaxHourPerDay : result;
        }
    }
}
