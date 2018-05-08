using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;
using Sofco.Model.Enums;
using Sofco.Model.Utils;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeService : IWorkTimeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IEmployeeData employeeData;

        private readonly ILogMailer<WorkTimeService> logger;

        public WorkTimeService(ILogMailer<WorkTimeService> logger, IUnitOfWork unitOfWork, IUserData userData, IEmployeeData employeeData)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.employeeData = employeeData;
            this.logger = logger;
        }

        public Response<WorkTimeModel> Get(DateTime date)
        {
            if(date == DateTime.MinValue) return new Response<WorkTimeModel>();

            var result = new Response<WorkTimeModel> {Data = new WorkTimeModel()};

            try
            {
                var currentUser = userData.GetCurrentUser();

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
            result.Data.Resume = new WorkTimeResumeModel
            {
                HoursApproved = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Approved).Sum(x => x.Hours),
                HoursRejected = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Rejected).Sum(x => x.Hours),
                HoursPending = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Draft).Sum(x => x.Hours),
                HoursPendingApproved =
                    result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Sent).Sum(x => x.Hours)
            };

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

        public Response<WorkTime> Save(WorkTimeAddModel model)
        {
            var response = new Response<WorkTime>();

            model.Date = model.Date.ToUniversalTime();

            SetCurrentUser(model);

            WorkTimeValidationHandler.ValidateStatus(response, model);
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

                unitOfWork.WorkTimeRepository.Save(workTime);

                unitOfWork.Save();

                response.Data = workTime;

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

        public Response<IList<HoursApprovedModel>> GetHoursPending(WorktimeHoursPendingParams model)
        {
            var response = new Response<IList<HoursApprovedModel>>();

            var currentUser = userData.GetCurrentUser();
            var userIsDirector = unitOfWork.UserRepository.HasDirectorGroup(currentUser.Email);
            var userIsManager = unitOfWork.UserRepository.HasManagerGroup(currentUser.UserName);

            var list = unitOfWork.WorkTimeRepository.SearchPending(model, userIsManager || userIsDirector, currentUser.Id);

            if (!list.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }

            response.Data = list.Select(x => new HoursApprovedModel(x)).ToList();

            return response;
        }

        public Response Approve(int id)
        {
            var response = new Response();

            var worktime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == id);

            WorkTimeValidationHandler.ValidateApproveOrReject(worktime, response);

            if (response.HasErrors()) return response;

            try
            {
                worktime.Status = WorkTimeStatus.Approved;
                worktime.ApprovalUserId = userData.GetCurrentUser().Id;

                unitOfWork.WorkTimeRepository.UpdateStatus(worktime);
                unitOfWork.Save();

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApprovedSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public IEnumerable<Option> GetAnalytics()
        {
            var currentUser = userData.GetCurrentUser();
            var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagers(currentUser.Id);
            var analyticsByDelegates = unitOfWork.WorkTimeApprovalRepository.GetByAnalyticApproval(currentUser.Id);

            var list = analyticsByManagers.Select(x => new Option {Id = x.Id, Text = $"{x.Title} - {x.Name}"}).ToList();

            foreach (var analyticsByDelegate in analyticsByDelegates)
            {
                if (list.All(x => x.Id != analyticsByDelegate.Id))
                {
                   list.Add(new Option { Id = analyticsByDelegate.Id, Text = $"{analyticsByDelegate.Title} - {analyticsByDelegate.Name}" }); 
                }
            }

            return list;
        }

        public Response Reject(int id, string comments)
        {
            var response = new Response();

            var worktime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == id);

            WorkTimeValidationHandler.ValidateApproveOrReject(worktime, response);

            if (response.HasErrors()) return response;

            try
            {
                worktime.Status = WorkTimeStatus.Rejected;
                worktime.ApprovalComment = comments;
                worktime.ApprovalUserId = userData.GetCurrentUser().Id;

                unitOfWork.WorkTimeRepository.UpdateStatus(worktime);
                unitOfWork.WorkTimeRepository.UpdateApprovalComment(worktime);
                unitOfWork.Save();

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.RejectedSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response ApproveAll(List<int> hourIds)
        {
            var response = new Response();
            var anyError = false;
            var anySuccess = false;

            foreach (var hourId in hourIds)
            {
                var hourResponse = Approve(hourId);

                if (hourResponse.HasErrors())
                    anyError = true;
                else
                    anySuccess = true;
            }

            if (anySuccess)
            {
                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApprovedSuccess);

                if (anyError)
                {
                    response.AddWarning(Resources.WorkTimeManagement.WorkTime.ApprovedWithSomeErrors);
                }
            }
            else
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Send()
        {
            var response = new Response();

            try
            {
                unitOfWork.WorkTimeRepository.SendHours(employeeData.GetCurrentEmployee().Id);
                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.SentSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void SetCurrentUser(WorkTimeAddModel workTimeAdd)
        {
            if (workTimeAdd.UserId > 0) return;

            var currentUser = userData.GetCurrentUser();

            if(currentUser == null) return;

            workTimeAdd.UserId = currentUser.Id;

            var currentEmployee = employeeData.GetCurrentEmployee();

            if (currentEmployee == null) return;

            workTimeAdd.EmployeeId = currentEmployee.Id;
        }
    }
}

