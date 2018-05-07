﻿using System;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Models.WorkTimeManagement;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.WorkTimeManagement
{
    public static class WorkTimeValidationHandler
    {
        private const int UserCommentMaxLength = 500;

        private const int AllowedHoursPerDay = 12;

        public static void ValidateEmployee(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.EmployeeId <= 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.EmployeeRequired);
            }
            else
            {
                if (!unitOfWork.EmployeeRepository.Exist(model.EmployeeId))
                {
                    response.AddError(Resources.AllocationManagement.Employee.NotFound);
                }
            }
        }

        public static void ValidateAnalytic(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.AnalyticId <= 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.AnalyticRequired);
            }
            else
            {
                if (!unitOfWork.AnalyticRepository.Exist(model.AnalyticId))
                {
                    response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                }
            }
        }

        public static void ValidateUser(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.UserId <= 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.UserRequired);
            }
            else
            {
                if (!unitOfWork.UserRepository.ExistById(model.UserId))
                {
                    response.AddError(Resources.Admin.User.NotFound);
                }
            }
        }

        public static void ValidateTask(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.TaskId <= 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.TaskRequired);
            }
            else
            {
                if (!unitOfWork.TaskRepository.ExistById(model.TaskId))
                {
                    response.AddError(Resources.Admin.Task.NotFound);
                }
            }
        }
        public static void ValidateHours(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.Hours < 1)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.HoursWrong);

                return;
            }

            var totalHours = unitOfWork.WorkTimeRepository.GetTotalHoursByDate(model.Date, model.UserId);

            if (totalHours + model.Hours > AllowedHoursPerDay)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.HoursMaxError);
            }
        }

        public static void ValidateDate(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.Date == DateTime.MinValue)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateRequired);
            }

            if (model.Date.Month != DateTime.UtcNow.Month)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateOutOfRangeError);
            }
        }

        public static void ValidateUserComment(Response<WorkTime> response, WorkTimeAddModel model)
        {
            if (model.UserComment.Length > UserCommentMaxLength)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.UserCommentMaxLengthError);
            }
        }

        public static void ValidateApproveOrReject(WorkTime worktime, Response response)
        {
            if (worktime == null)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.WorkTimeNotFound);
            }
            else
            {
                if (worktime.Status != WorkTimeStatus.Sent)
                {
                    response.AddError(Resources.WorkTimeManagement.WorkTime.CannotChangeStatus);
                }
            }
        }
    }
}
