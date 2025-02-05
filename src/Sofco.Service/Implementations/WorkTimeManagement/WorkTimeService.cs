﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Managers;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Core.Validations;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sofco.Service.Implementations.WorkTimeManagement
{
    public class WorkTimeService : IWorkTimeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IUserData userData;

        private readonly IEmployeeData employeeData;

        private readonly ILogMailer<WorkTimeService> logger;

        private readonly IWorkTimeValidation workTimeValidation;

        private readonly IWorkTimeImportFileManager workTimeImportFileManager;

        private readonly IWorkTimeExportFileManager workTimeExportFileManager;

        private readonly IWorkTimeResumeManager workTimeResumeManger;

        private readonly IWorkTimeRejectManager workTimeRejectManager;

        private readonly IWorkTimeSendManager workTimeSendHoursManager;

        private readonly AppSetting appSetting;

        private readonly IRoleManager roleManager;

        public WorkTimeService(ILogMailer<WorkTimeService> logger,
            IUnitOfWork unitOfWork,
            IUserData userData,
            IEmployeeData employeeData,
            IWorkTimeValidation workTimeValidation,
            IOptions<AppSetting> appSetting,
            IRoleManager roleManager,
            IWorkTimeImportFileManager workTimeImportFileManager,
            IWorkTimeExportFileManager workTimeExportFileManager,
            IWorkTimeResumeManager workTimeResumeManger,
            IWorkTimeRejectManager workTimeRejectManager, IWorkTimeSendManager workTimeSendHoursManager)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.employeeData = employeeData;
            this.workTimeValidation = workTimeValidation;
            this.logger = logger;
            this.workTimeImportFileManager = workTimeImportFileManager;
            this.workTimeResumeManger = workTimeResumeManger;
            this.workTimeExportFileManager = workTimeExportFileManager;
            this.workTimeRejectManager = workTimeRejectManager;
            this.workTimeSendHoursManager = workTimeSendHoursManager;
            this.appSetting = appSetting.Value;
            this.roleManager = roleManager;
        }

        public Response<WorkTimeModel> Get(DateTime date)
        {
            if (date == DateTime.MinValue) return new Response<WorkTimeModel>();

            var result = new Response<WorkTimeModel> { Data = new WorkTimeModel() };

            try
            {
                var currentUser = userData.GetCurrentUser();

                var startDate = new DateTime(date.Year, date.Month, 1);

                var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

                var workTimes = unitOfWork.WorkTimeRepository.Get(startDate, endDate, currentUser.Id);

                var calendars = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

                result.Data.Calendar = calendars;

                var dateUtc = date.ToUniversalTime();

                result.Data.Holidays = unitOfWork.HolidayRepository.Get(dateUtc.Year, dateUtc.Month);

                result.Data.Resume = workTimeResumeManger.GetCurrentPeriodResume();

                var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

                var period = closeDates.GetPeriodIncludeDays();

                result.Data.PeriodStartDate = period.Item1;
                result.Data.PeriodEndDate = period.Item2;

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e);

                result.AddError(Resources.Common.GeneralError);

                return result;
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
            workTimeValidation.ValidateDate(response, model);
            WorkTimeValidationHandler.ValidateTask(response, unitOfWork, model);
            WorkTimeValidationHandler.ValidateUserComment(response, model);
            workTimeValidation.ValidateHours(response, model);
            workTimeValidation.ValidateAllocations(response, model);

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

            if (response.HasErrors()) return response;

            model.AnalyticIds = GetAnalyticIds(model.AnalyticId);

            var list = unitOfWork.WorkTimeRepository.SearchApproved(model).ToList();

            list.AddRange(AddDelegatedData(model.AnalyticId, model.EmployeeId, WorkTimeStatus.Approved, list));

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

            var list = unitOfWork.WorkTimeRepository
                .SearchPending(model, userIsManager || userIsDirector, currentUser.Id, appSetting.Analytic999)
                .ToList();

            var delegatedData = AddDelegatedData(model.AnalyticId, model.EmployeeId, WorkTimeStatus.Sent, list);

            foreach (var workTime in delegatedData)
            {
                if (list.All(x => x.Id != workTime.Id))
                {
                    list.Add(workTime);
                }
            }

            if (!list.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }

            response.Data = new List<HoursApprovedModel>();

            foreach (var workTime in list)
            {
                var itemToAdd = new HoursApprovedModel(workTime);
                response.Data.Add(itemToAdd);
            }

            return response;
        }

        public Response Approve(int id)
        {
            var response = new Response();

            var worktime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == id);

            try
            {
                WorkTimeValidationHandler.ValidateApproveOrReject(worktime, response);

                if (response.HasErrors()) return response;

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
            if (roleManager.IsPmo() || roleManager.IsRrhh() || roleManager.IsCdg())
            {
                var analytics = unitOfWork.AnalyticRepository.GetAllOpenAnalyticLite();
                var list = new List<Option>();

                foreach (var analytic in analytics)
                {
                    var accountName = string.Empty;

                    if (!string.IsNullOrWhiteSpace(analytic.AccountId))
                    {
                        accountName = analytic.AccountName + " - ";
                    }

                    list.Add(new Option { Id = analytic.Id, Text = $"{accountName}{analytic.Title} - {analytic.Name}" });
                }

                return list;
            }
            else
            {
                var currentUser = userData.GetCurrentUser();
                var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);
                var analyticsByDirectors = unitOfWork.AnalyticRepository.GetByDirectorId(currentUser.Id);

                var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.WorkTime);

                var list = new List<Option>();

                foreach (var analytic in analyticsByManagers)
                {
                    var accountName = string.Empty;

                    if (!string.IsNullOrWhiteSpace(analytic.AccountId))
                    {
                        accountName = analytic.AccountName + " - ";
                    }

                    list.Add(new Option { Id = analytic.Id, Text = $"{accountName}{analytic.Title} - {analytic.Name}" });
                }

                foreach (var analytic in analyticsByDirectors)
                {
                    var accountName = string.Empty;

                    if (!string.IsNullOrWhiteSpace(analytic.AccountId))
                    {
                        accountName = analytic.AccountName + " - ";
                    }

                    if (list.All(x => x.Id != analytic.Id))
                    {
                        list.Add(new Option { Id = analytic.Id, Text = $"{accountName}{analytic.Title} - {analytic.Name}" });
                    }
                }

                foreach (var delegation in delegations)
                {
                    if (list.All(x => x.Id != delegation.AnalyticSourceId))
                    {
                        var analytic = unitOfWork.AnalyticRepository.Get(delegation.AnalyticSourceId.GetValueOrDefault());

                        list.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
                    }
                }

                return list;
            }
        }

        public Response Reject(int id, string comments, bool massive)
        {
            return workTimeRejectManager.Reject(id, comments, massive);
        }

        public Response ApproveAll(List<HoursToApproveModel> hours)
        {
            var response = new Response();
            var anyError = false;
            var anySuccess = false;

            foreach (var hour in hours)
            {
                var hourResponse = Approve(hour.Id);

                if (hourResponse.HasErrors())
                    anyError = true;
                else
                    anySuccess = true;
            }

            if (anySuccess && anyError)
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.ApprovedWithSomeErrors);
            }

            if (anySuccess)
            {
                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.ApprovedSuccess);
            }
            else
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Send()
        {
            return workTimeSendHoursManager.Send();
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

            response.Data = new List<WorkTimeSearchItemResult>();

            var analytics = GetAnalytics();

            if (!analytics.Any()) return response;

            var analyticIds = analytics.Select(x => x.Id).ToList();

            var worktimes = unitOfWork.WorkTimeRepository.Search(parameters, analyticIds);

            var currentUser = userData.GetCurrentUser();

            var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.WorkTime);

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();
            var period = closeDates.GetPeriodIncludeDays();

            var startDate = period.Item1;

            var endDate = period.Item2;

            foreach (var worktime in worktimes)
            {
                var model = new WorkTimeSearchItemResult();

                var delegationsByAnalytic = delegations.Where(x => x.AnalyticSourceId == worktime.AnalyticId).ToList();

                if (delegationsByAnalytic.Any())
                {
                    if (delegationsByAnalytic.Where(x => x.EmployeeSourceId.HasValue).All(x => x.EmployeeSourceId != worktime.EmployeeId))
                        continue;
                }

                if (worktime.Analytic != null)
                {
                    model.Client = worktime.Analytic.AccountName;
                    model.AnalyticTitle = worktime.Analytic.Title;
                    model.Analytic = worktime.Analytic.Name;
                    model.Manager = worktime.Analytic?.Manager?.Name;
                    model.AnalyticId = worktime.AnalyticId;
                }

                if (worktime.Employee != null)
                {
                    model.Employee = $"{worktime.Employee?.EmployeeNumber} - {worktime.Employee.Name}";
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
                model.Reference = worktime.Reference;
                model.Comments = worktime.UserComment;
                model.Status = worktime.Status.ToString();
                model.StatusId = (int)worktime.Status;
                model.Id = worktime.Id;

                if (roleManager.IsAdmin())
                {
                    model.CanDelete = true;
                }
                else
                {
                    if (!roleManager.IsPmo() && !roleManager.IsRrhh() && !roleManager.IsCdg())
                    {
                        if (model.Date.Date >= startDate.Date && model.Date.Date <= endDate.Date && worktime.Status != WorkTimeStatus.License)
                        {
                            model.CanDelete = true;
                        }
                    }
                }
  
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
                var hourResponse = Reject(hourId, parameters.Comments, true);

                if (hourResponse.HasErrors())
                    anyError = true;
                else
                    anySuccess = true;
            }

            if (anySuccess && anyError)
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.RejectedWithSomeErrors);
            }

            if (anySuccess)
            {
                var workTime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == parameters.HourIds.FirstOrDefault());

                workTimeRejectManager.SendGeneralRejectMail(workTime);
                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.RejectedSuccess);
            }
            else
            {
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var workTime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == id);

            workTimeValidation.ValidateDelete(response, workTime);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.WorkTimeRepository.Delete(workTime);
                unitOfWork.Save();

                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.DeleteSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.WorkTimeManagement.WorkTime.DeleteError);
            }

            return response;
        }

        public void Import(int analyticId, IFormFile file, Response<IList<WorkTimeImportResult>> response)
        {
            AnalyticValidationHelper.Exist(response, unitOfWork.AnalyticRepository, analyticId);

            if (response.HasErrors()) return;

            var memoryStream = new MemoryStream();
            file.CopyTo(memoryStream);

            workTimeImportFileManager.Import(analyticId, memoryStream, response);
        }

        public byte[] ExportTemplate(int analyticId, int periodId)
        {
            var excel = workTimeExportFileManager.CreateTemplateExcel(analyticId, periodId);

            return excel.GetAsByteArray();
        }

        public Response AdminUpdate(int id, AdminUpdateParams request)
        {
            var response = new Response();

            var worktime = unitOfWork.WorkTimeRepository.Get(id);

            if (worktime == null)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.WorkTimeNotFound);
                return response;
            }

            try
            {
                worktime.AnalyticId = request.AnalyticId;
                worktime.Status = (WorkTimeStatus) request.StatusId;

                unitOfWork.WorkTimeRepository.AdminUpdate(worktime);
                unitOfWork.Save();

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
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

        private void SetCurrentUser(WorkTimeAddModel workTimeAdd)
        {
            if (workTimeAdd.UserId > 0) return;

            var currentUser = userData.GetCurrentUser();

            if (currentUser == null) return;

            workTimeAdd.UserId = currentUser.Id;

            var currentEmployee = employeeData.GetCurrentEmployee();

            if (currentEmployee == null) return;

            workTimeAdd.EmployeeId = currentEmployee.Id;
        }

        private List<int> GetAnalyticIds(int? analyticId)
        {
            var currentUser = userData.GetCurrentUser();

            var availableAnalyticIds = unitOfWork.AnalyticRepository.GetAnalyticLiteByManagerId(currentUser.Id).Select(s => s.Id).ToList();

            if (!analyticId.HasValue || analyticId == 0) return availableAnalyticIds;

            var selectedAnalyticId = analyticId.Value;

            return availableAnalyticIds.Contains(selectedAnalyticId)
                ? new List<int> { selectedAnalyticId }
                : new List<int>();
        }

        private List<WorkTime> AddDelegatedData(int? analyticId, int? employeeId, WorkTimeStatus status, List<WorkTime> workTimes)
        {
            var result = new List<WorkTime>();

            var currentUser = userData.GetCurrentUser();

            var delegations = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.WorkTime);

            if (analyticId.HasValue && analyticId.Value > 0)
            {
                delegations = delegations
                    .Where(s => s.AnalyticSourceId == analyticId.Value)
                    .ToList();
            }

            if (employeeId.HasValue && employeeId.Value > 0)
            {
                delegations = delegations
                    .Where(s => s.EmployeeSourceId == employeeId.Value)
                    .ToList();
            }

            if (!delegations.Any()) return result;

            var employeeIds = delegations.Select(s => s.EmployeeSourceId.GetValueOrDefault()).ToList();

            var analyticIds = delegations.Select(x => x.AnalyticSourceId.GetValueOrDefault()).ToList();

            var alreadyLoadedEmployeeIds = workTimes.Select(s => s.EmployeeId).Distinct();

            employeeIds.RemoveAll(s => alreadyLoadedEmployeeIds.Contains(s));

            result = unitOfWork
                .WorkTimeRepository
                .GetByEmployeeIds(employeeIds, analyticIds, status)
                .ToList();

            if (status != WorkTimeStatus.Approved)
            {
                if (analyticId.HasValue && analyticId.Value > 0)
                {
                    result = result.Where(s => s.Status == status && s.AnalyticId == analyticId.Value).ToList();
                }
                else
                {
                    result = result.Where(s => s.Status == status).ToList();
                }
            }
            else
            {
                if (analyticId.HasValue && analyticId.Value > 0)
                {
                    result = result.Where(s => (s.Status == status || s.Status == WorkTimeStatus.License) && s.AnalyticId == analyticId.Value).ToList();
                }
                else
                {
                    result = result.Where(s => s.Status == status || s.Status == WorkTimeStatus.License).ToList();
                }
            }

            return result;
        }
    }
}

