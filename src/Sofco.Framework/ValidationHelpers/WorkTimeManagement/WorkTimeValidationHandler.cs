using System;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Framework.ValidationHelpers.WorkTimeManagement
{
    public static class WorkTimeValidationHandler
    {
        private const int UserCommentMaxLength = 500;

        private static bool ValidatePeriodCloseMonth = true;

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

        public static void ValidateDate(Response<WorkTime> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
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

            if (!ValidatePeriodCloseMonth) return; 
             
            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            // Item 1: DateFrom
            // Item 2: DateTo
            var period = closeDates.GetPeriodIncludeDays();

            if (!(model.Date.Date >= period.Item1.Date && model.Date.Date <= period.Item2.Date))
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

        public static void ValidateStatus(Response<WorkTime> response, WorkTimeAddModel model)
        {
            if (model.Status == 0) return;

            if (model.Status != WorkTimeStatus.Draft && model.Status != WorkTimeStatus.Rejected)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.WrongStatus);
            }
        }

        public static void ValidateDelete(WorkTime worktime, Response response, IUnitOfWork unitOfWork)
        {
            if (worktime == null)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.WorkTimeNotFound);
                return;
            }

            if (!ValidatePeriodCloseMonth) return;

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            // Item 1: DateFrom
            // Item 2: DateTo
            var period = closeDates.GetPeriodIncludeDays();

            if (!(worktime.Date.Date >= period.Item1.Date && worktime.Date.Date <= period.Item2.Date))
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.CannotDelete);
            }
        }
    }
}
