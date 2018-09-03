﻿using Sofco.Core.Data.Admin;
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
    }
}
