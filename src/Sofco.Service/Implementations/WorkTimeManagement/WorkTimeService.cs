using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Data.Admin;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Framework.ValidationHelpers.WorkTimeManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.FileManager;
using Sofco.Core.Managers;
using Sofco.Core.Validations;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.ValidationHelpers.AllocationManagement;

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

        public WorkTimeService(ILogMailer<WorkTimeService> logger,
            IUnitOfWork unitOfWork,
            IUserData userData,
            IEmployeeData employeeData,
            IWorkTimeValidation workTimeValidation,
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

            var list = unitOfWork.WorkTimeRepository.SearchApproved(model);

            if (!list.Any())
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.SearchNotFound);
            }

            list = ResolveDelegateResource(list.ToList());

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

            list = ResolveDelegateResource(list.ToList());

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
            var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);
            var analyticsByDelegates = unitOfWork.UserApproverRepository.GetByAnalyticApprover(currentUser.Id, UserApproverType.WorkTime);

            var list = analyticsByManagers.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();

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
            return workTimeRejectManager.Reject(id, comments);
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

            var worktimes = unitOfWork.WorkTimeRepository.Search(parameters);

            response.Data = new List<WorkTimeSearchItemResult>();

            foreach (var worktime in worktimes)
            {
                var model = new WorkTimeSearchItemResult();

                if (worktime.Analytic != null)
                {
                    model.Client = worktime.Analytic.ClientExternalName;
                    model.Analytic = $"{worktime.Analytic.Name} - {worktime.Analytic.Service}";
                    model.Manager = worktime.Analytic?.Manager?.Name;
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

            if (anySuccess && anyError)
            {
                response.AddWarning(Resources.WorkTimeManagement.WorkTime.RejectedWithSomeErrors);
            }

            if (anySuccess)
            {
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

        public byte[] ExportTemplate(int analyticId)
        {
            var excel = workTimeExportFileManager.CreateTemplateExcel(analyticId);

            return excel.GetAsByteArray();
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
            if (analyticId.HasValue && analyticId != 0) return new List<int> { analyticId.Value };

            var currentUser = userData.GetCurrentUser();

            return unitOfWork.AnalyticRepository.GetAnalyticLiteByManagerId(currentUser.Id).Select(s => s.Id).ToList();
        }

        private List<WorkTime> ResolveDelegateResource(List<WorkTime> workTimes)
        {
            var currentUser = userData.GetCurrentUser();

            var userApprovers =
                unitOfWork.UserApproverRepository.GetByApproverUserId(currentUser.Id, UserApproverType.WorkTime);

            if (!userApprovers.Any()) return workTimes;

            var assignedEmployeeIds = userApprovers.Select(s => s.EmployeeId).ToList();

            return workTimes.Where(s => assignedEmployeeIds.Contains(s.EmployeeId)).ToList();
        }
    }
}

