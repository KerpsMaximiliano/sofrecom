using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Core.DAL;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.WorkTimeManagement
{
    public static class WorkTimeValidationHandler
    {
        public static void ValidateEmployee(Response<WorkTimeAddModel> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
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

        public static void ValidateAnalytic(Response<WorkTimeAddModel> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
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

        public static void ValidateUser(Response<WorkTimeAddModel> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
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

        public static void ValidateTask(Response<WorkTimeAddModel> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.TaskId <= 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.TaskRequired);
            }
            else
            {
                if (!unitOfWork.TaskRepository.ExistById(model.UserId))
                {
                    response.AddError(Resources.Admin.Task.NotFound);
                }
            }
        }
        public static void ValidateHours(Response<WorkTimeAddModel> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.Hours < 1 || model.Hours > 12)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.HoursWrong);
            }
        }

        public static void ValidateDate(Response<WorkTimeAddModel> response, IUnitOfWork unitOfWork, WorkTimeAddModel model)
        {
            if (model.Date == DateTime.MinValue)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DateRequired);
            }
        }
    }
}
