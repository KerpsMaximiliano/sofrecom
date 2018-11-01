﻿using System;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Validations;
using Sofco.Domain.Utils;

namespace Sofco.Framework.Validations.WorkTimeManagement
{
    public class WorkTimeValidation : IWorkTimeValidation
    {
        private const string WorkingHoursPerDaysMaxKey = "WorkingHoursPerDaysMax";

        private readonly int allowedHoursPerDay = 12;

        private readonly IUnitOfWork unitOfWork;

        public WorkTimeValidation(IUnitOfWork unitOfWork, ISettingData settingData)
        {
            this.unitOfWork = unitOfWork;
            var setting  = settingData.GetByKey(WorkingHoursPerDaysMaxKey);
            if (setting != null)
            {
                allowedHoursPerDay = int.Parse(setting.Value);
            }
        }

        public void ValidateHours(Response response, WorkTimeAddModel model)
        {
            if (model.Hours < (decimal) 0.25)
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
    }
}
