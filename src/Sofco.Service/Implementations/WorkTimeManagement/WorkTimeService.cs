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
using Sofco.Core.Validations;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Models.WorkTimeManagement;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeService : IWorkTimeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IEmployeeData employeeData;

        private readonly ILogMailer<WorkTimeService> logger;

        private readonly IWorkTimeValidation workTimeValidation;

        public WorkTimeService(ILogMailer<WorkTimeService> logger, IUnitOfWork unitOfWork, IUserData userData, IEmployeeData employeeData, IWorkTimeValidation workTimeValidation)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.employeeData = employeeData;
            this.workTimeValidation = workTimeValidation;
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

                var dateUtc = date.ToUniversalTime();

                result.Data.Holidays = unitOfWork.HolidayRepository.Get(dateUtc.Year, dateUtc.Month);

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
                HoursPendingApproved = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.Sent).Sum(x => x.Hours),
                HoursWithLicense = result.Data.Calendar.Where(x => x.Status == WorkTimeStatus.License).Sum(x => x.Hours)
            };

            while (startDate.Date <= endDate.Date)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                {
                    result.Data.Resume.BusinessHours += 8;

                    if (startDate.Date <= DateTime.UtcNow.Date)
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
            WorkTimeValidationHandler.ValidateTask(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateUserComment(response, model);
            workTimeValidation.ValidateHours(response, model);

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

            var list = unitOfWork.WorkTimeRepository.SearchApproved(model);

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

        public Response<IList<WorkTimeReportModel>> CreateReport(ReportParams parameters)
        {
            var response = new Response<IList<WorkTimeReportModel>>();

            if (parameters.Year == 0 || parameters.Month == 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.YearAndMonthRequired);
                return response;
            }

            var allocations = unitOfWork.AllocationRepository.GetAllocationsForWorktimeReport(parameters);

            response.Data = new List<WorkTimeReportModel>();

            foreach (var allocation in allocations)
            {
                var model = new WorkTimeReportModel();

                if (allocation.Analytic == null || allocation.Employee == null || allocation.Analytic.Manager == null)
                    continue;

                if(allocation.Percentage == 0) continue;

                model.Client = allocation.Analytic.ClientExternalName;
                model.Analytic = $"{allocation.Analytic.Name} - {allocation.Analytic.Service}";
                model.Manager = allocation.Analytic.Manager.Name;
                model.Employee = allocation.Employee.Name;
                model.MonthYear = $"{parameters.Month}-{parameters.Year}";
                model.Facturability = allocation.Employee.BillingPercentage;
                model.AllocationPercentage = allocation.Percentage;
                model.HoursMustLoad = CalculateHoursToLoad(allocation);
                model.HoursLoaded = unitOfWork.WorkTimeRepository.GetTotalHoursBetweenDays(allocation.EmployeeId, allocation.StartDate, allocation.AnalyticId);

                model.Result = model.HoursLoaded >= model.HoursMustLoad;

                response.Data.Add(model);
            }

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }

            return response;
        }

        public Response<IList<WorkTimeSearchItemResult>> Search(SearchParams parameters)
        {
            var response = new Response<IList<WorkTimeSearchItemResult>>();

            if (!parameters.StartDate.HasValue || parameters.StartDate == DateTime.MinValue || 
                !parameters.EndDate.HasValue || parameters.EndDate == DateTime.MinValue)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.DatesRequired);
                return response;
            }

            var worktimes = unitOfWork.WorkTimeRepository.Search(parameters);

            response.Data = new List<WorkTimeSearchItemResult>();

            foreach (var worktime in worktimes)
            {
                var model = new WorkTimeSearchItemResult();

                if (worktime.Analytic != null)
                {
                    model.Client = worktime.Analytic.ClientExternalName;
                    model.Analytic = $"{worktime.Analytic.Name} - {worktime.Analytic.Service}";
                    model.Manager = worktime.Analytic.Manager.Name;
                }

                if (worktime.Employee != null)
                {
                    model.Employee = $"{worktime.Employee.EmployeeNumber} - {worktime.Employee.Name}";
                    model.Profile = worktime.Employee.Profile;
                }

                if (worktime.Task != null)
                {
                    model.Task = worktime.Task.Description;

                    if (worktime.Task.Category != null)
                    {
                        model.Category = worktime.Task.Category.Description;
                    }
                }
               
                model.Date = worktime.Date;
                model.Hours = worktime.Hours;
                model.Status = worktime.Status.ToString();

                response.Data.Add(model);
            }

            if (!response.Data.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }

            return response;
        }

        public Response RejectAll(WorkTimeRejectParams parameters)
        {
            var response = new Response();
            var anyError = false;
            var anySuccess = false;

            foreach (var hourId in parameters.HourIds)
            {
                var hourResponse = Reject(hourId, parameters.Comments);

                if (hourResponse.HasErrors())
                    anyError = true;
                else
                    anySuccess = true;
            }

            if (anySuccess)
            {
                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.RejectedSuccess);

                if (anyError)
                {
                    response.AddWarning(Resources.WorkTimeManagement.WorkTime.RejectedWithSomeErrors);
                }
            }
            else
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public IEnumerable<Option> GetStatus()
        {
            yield return new Option { Id = (int)WorkTimeStatus.Draft, Text = WorkTimeStatus.Draft.ToString() };
            yield return new Option { Id = (int)WorkTimeStatus.Sent, Text = WorkTimeStatus.Sent.ToString() };
            yield return new Option { Id = (int)WorkTimeStatus.Rejected, Text = WorkTimeStatus.Rejected.ToString() };
            yield return new Option { Id = (int)WorkTimeStatus.Approved, Text = WorkTimeStatus.Approved.ToString() };
            yield return new Option { Id = (int)WorkTimeStatus.License, Text = WorkTimeStatus.License.ToString() };
        }

        private decimal CalculateHoursToLoad(Allocation allocation)
        {
            var startDate = allocation.StartDate.Date;
            var endDate = new DateTime(allocation.StartDate.Year, allocation.StartDate.Month, DateTime.DaysInMonth(allocation.StartDate.Year, allocation.StartDate.Month));
            var businessDays = 0;

            while (startDate.Date <= endDate.Date)
            {
                if (startDate.DayOfWeek != DayOfWeek.Saturday && startDate.DayOfWeek != DayOfWeek.Sunday)
                    businessDays++;

                startDate = startDate.AddDays(1);
            }

            return Math.Round((businessDays * allocation.Employee.BusinessHours * allocation.Percentage) / 100);
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

