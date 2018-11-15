using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Validations;
using Sofco.Domain;
using Sofco.Domain.Models.AllocationManagement;
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

            if(!ValidateAllocation(model.Date, model.AnalyticId, allocations.ToList()))
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.InvalidAllocationAssignment);
            }
        }

        public void ValidateAllocations(Response response, List<WorkTime> workTimes)
        {
            if (!workTimes.Any())
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.NoPendingHours);
                return;
            }

            var employeeId = workTimes.First().EmployeeId;

            var allocations = unitOfWork.AllocationRepository.GetByEmployee(employeeId);

            foreach (var workTime in workTimes)
            {
                if(response.HasErrors()) continue;

                if (!ValidateAllocation(workTime.Date, workTime.AnalyticId, allocations.ToList()))
                {
                    response.AddError(Resources.WorkTimeManagement.WorkTime.InvalidAllocationAssignment);
                }
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

        private bool ValidateAllocation(DateTime workTimeDate, int analyticId, List<Allocation> allocations)
        {
            var modelDate = new DateTime(workTimeDate.Year, workTimeDate.Month, 1);

            var valid = allocations.Any(s => s.AnalyticId == analyticId
                                             && s.StartDate.Date == modelDate
                                             && workTimeDate.Date <= s.ReleaseDate.Date
                                             && s.Percentage > 0);
            return valid;
        }
    }
}
