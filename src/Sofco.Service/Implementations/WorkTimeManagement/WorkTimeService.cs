using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Core.Data.AllocationManagement;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeService : IWorkTimeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IEmployeeData employeeData;

        private readonly ISessionManager sessionManager;

        private readonly ILogMailer<WorkTimeService> logger;

        public WorkTimeService(ISessionManager sessionManager, ILogMailer<WorkTimeService> logger, IUnitOfWork unitOfWork, IUserData userData, IEmployeeData employeeData)
        {
            this.sessionManager = sessionManager;
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.employeeData = employeeData;
            this.logger = logger;
        }

        public Response<WorkTimeModel> Get(DateTime date)
        {
            if(date == DateTime.MinValue) return new Response<WorkTimeModel>();

            var result = new Response<WorkTimeModel>(); 
            result.Data = new WorkTimeModel();

            try
            {
                var userName = sessionManager.GetUserName();
                var currentUser = userData.GetByUserName(userName);

                var startDate = new DateTime(date.Year, date.Month, 1);
                var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

                var workTimes = unitOfWork.WorkTimeRepository.Get(startDate.Date, endDate.Date, currentUser.Id);

                result.Data.Calendar = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

                FillResume(result, startDate, endDate);

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e);

                result.AddError(Resources.Common.GeneralError);

                return result;
            }
        }

        private void FillResume(Response<WorkTimeModel> result, DateTime startDate, DateTime endDate)
        {
            result.Data.Resume = new WorkTimeResumeModel();

            result.Data.Resume.HoursApproved = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Approved).Sum(x => x.Hours);
            result.Data.Resume.HoursRejected = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Rejected).Sum(x => x.Hours);
            result.Data.Resume.HoursPending = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Draft).Sum(x => x.Hours);
            result.Data.Resume.HoursPendingApproved = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Sent).Sum(x => x.Hours);

            while (startDate.Date <= endDate.Date)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    result.Data.Resume.BusinessHours += 8;

                    if (startDate.Date <= DateTime.UtcNow)
                    {
                        result.Data.Resume.HoursUntilToday += 8;
                    }
                }

                startDate = startDate.AddDays(1);
            }
        }

        public Response<WorkTimeAddModel> Add(WorkTimeAddModel model)
        {
            var response = new Response<WorkTimeAddModel>();

            SetCurrentUser(model);

            WorkTimeValidationHandler.ValidateEmployee(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateAnalytic(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateUser(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateDate(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateHours(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateTask(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateUserComment(response, model);

            if (response.HasErrors()) return response;

            try
            {
                var workTime = model.CreateDomain();

                unitOfWork.WorkTimeRepository.Insert(workTime);
                unitOfWork.Save();

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<HoursApprovedModel>> GetHoursApproved(WorktimeHoursApprovedParams model)
        {
            var response = new Response<IList<HoursApprovedModel>>();

            if (model.Month.HasValue && model.Month > 0)
            {
                if (model.Month < 1 || model.Month > 12)
                {
                    response.AddError(Resources.WorkTimeManagement.WorkTime.MonthError);
                }
            }

            if (model.Year.HasValue && model.Year > 0)
            {
                if (model.Year < 2015 || model.Year > 2050)
                {
                    response.AddError(Resources.WorkTimeManagement.WorkTime.YearError);
                }
            }

            if (response.HasErrors()) return response;

            var list = unitOfWork.WorkTimeRepository.Search(model);

            if (!list.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }

            response.Data = list.Select(x => new HoursApprovedModel(x)).ToList();

            return response;
        }

        private void SetCurrentUser(WorkTimeAddModel workTimeAdd)
        {
            if (workTimeAdd.UserId > 0) return;

            var currentUser = userData.GetByUserName(sessionManager.GetUserName());

            if(currentUser == null) return;

            workTimeAdd.UserId = currentUser.Id;

            var currentEmployee = employeeData.GetByEmail(currentUser.Email);

            if (currentEmployee == null) return;

            workTimeAdd.EmployeeId = currentEmployee.Id;
        }
    }
}

