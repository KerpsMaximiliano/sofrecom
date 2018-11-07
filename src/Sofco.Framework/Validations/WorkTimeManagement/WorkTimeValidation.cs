using System;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Validations;
using Sofco.Domain;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.WorkTimeManagement
{
    public class WorkTimeValidation : IWorkTimeValidation
    {
        private const decimal MinHours = (decimal)0.25;

        private readonly int allowedHoursPerDay = 12;

        private readonly IUnitOfWork unitOfWork;

        private readonly bool validatePeriodCloseMonth;

        public WorkTimeValidation(IUnitOfWork unitOfWork, ISettingData settingData)
        {
            this.unitOfWork = unitOfWork;
            var workingHoursSetting  = settingData.GetByKey(SettingConstant.WorkingHoursPerDaysMaxKey);
            if (workingHoursSetting != null)
            {
                allowedHoursPerDay = int.Parse(workingHoursSetting.Value);
            }
            var validatePeriodSetting = settingData.GetByKey(SettingConstant.ValidatePeriodCloseMonthKey);
            if (validatePeriodSetting != null)
            {
                validatePeriodCloseMonth = bool.Parse(validatePeriodSetting.Value);
            }
        }

        public void ValidateHours(Response response, WorkTimeAddModel model)
        {
            if (model.Hours < MinHours)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.HoursWrong);

                return;
            }

            var totalHours = unitOfWork.WorkTimeRepository.GetTotalHoursByDateExceptCurrentId(model.Date, model.UserId, model.Id);

            totalHours += model.Hours;

            if (totalHours > allowedHoursPerDay)
            {
                response.AddError(string.Format(Resources.WorkTimeManagement.WorkTime.HoursMaxError, allowedHoursPerDay));
            }
        }

        public void ValidateAllocations(Response response, WorkTimeAddModel model)
        {
            var allocations = unitOfWork.AllocationRepository.GetByEmployee(model.EmployeeId);

            var modelDate = new DateTime(model.Date.Year, model.Date.Month, 1);

            var valid = allocations.Any(s => s.AnalyticId == model.AnalyticId
                                             && s.StartDate.Date == modelDate
                                             && model.Date.Date <= s.ReleaseDate.Date
                                             && s.Percentage > 0);

            if (!valid)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.InvalidAllocationAssignment);
            }
        }

        public void ValidateDate(Response response, WorkTimeAddModel model)
        {
            if (model.Date == DateTime.MinValue)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateRequired);
            }

            if (model.Date.DayOfWeek == DayOfWeek.Saturday || model.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateIsWeekend);
            }

            if (unitOfWork.HolidayRepository.IsHoliday(model.Date))
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateIsHoliday);
            }

            if (!validatePeriodCloseMonth) return;

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            // Item 1: DateFrom
            // Item 2: DateTo
            var period = closeDates.GetPeriodIncludeDays();

            if (!(model.Date.Date >= period.Item1.Date && model.Date.Date <= period.Item2.Date))
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateOutOfRangeError);
            }
        }

        public void ValidateDelete(Response response, WorkTime workTime)
        {
            if (workTime == null)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.WorkTimeNotFound);
                return;
            }

            if (!validatePeriodCloseMonth) return;

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            // Item 1: DateFrom
            // Item 2: DateTo
            var period = closeDates.GetPeriodIncludeDays();

            if (!(workTime.Date.Date >= period.Item1.Date && workTime.Date.Date <= period.Item2.Date))
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.CannotDelete);
            }
        }
    }
}
