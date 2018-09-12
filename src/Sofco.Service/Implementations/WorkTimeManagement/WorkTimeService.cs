using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
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
using Sofco.Core.Mail;
using Sofco.Core.Validations;
using Sofco.Domain;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.MailData;
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

        private readonly IWorkTimeFileManager workTimeFileManager;

        private readonly IWorkTimeResumeManager workTimeResumeManger;

        private readonly IHostingEnvironment hostingEnvironment;

        private readonly IMailSender mailSender;

        private readonly IMailBuilder mailBuilder;

        public WorkTimeService(ILogMailer<WorkTimeService> logger, 
            IUnitOfWork unitOfWork, 
            IUserData userData,
            IHostingEnvironment hostingEnvironment,
            IEmployeeData employeeData, 
            IWorkTimeValidation workTimeValidation,
            IWorkTimeFileManager workTimeFileManager, 
            IWorkTimeResumeManager workTimeResumeManger,
            IMailSender mailSender,
            IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.userData = userData;
            this.employeeData = employeeData;
            this.workTimeValidation = workTimeValidation;
            this.logger = logger;
            this.workTimeFileManager = workTimeFileManager;
            this.workTimeResumeManger = workTimeResumeManger;
            this.hostingEnvironment = hostingEnvironment;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response<WorkTimeModel> Get(DateTime date)
        {
            if(date == DateTime.MinValue) return new Response<WorkTimeModel>();

            var result = new Response<WorkTimeModel> {Data = new WorkTimeModel()};

            try
            {
                var currentUser = userData.GetCurrentUser();

                var startDate = new DateTime(date.Year, date.Month, 1);

                var endDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

                var workTimes = unitOfWork.WorkTimeRepository.Get(startDate, endDate, currentUser.Id);

                var calendars = workTimes.Select(x => new WorkTimeCalendarModel(x)).ToList();

                result.Data.Calendar = calendars;

                var resumeModel = workTimeResumeManger.GetResume(calendars, startDate, endDate);

                var dateUtc = date.ToUniversalTime();

                result.Data.Holidays = unitOfWork.HolidayRepository.Get(dateUtc.Year, dateUtc.Month);

                result.Data.Resume = resumeModel;

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
            var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);
            var analyticsByDelegates = unitOfWork.UserApproverRepository.GetByAnalyticApprover(currentUser.Id, UserApproverType.WorkTime);

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
            var response = new Response();

            var user = userData.GetCurrentUser();
            var isManager = unitOfWork.UserRepository.HasManagerGroup(user.UserName);

            try
            {
                if (isManager)
                {
                    unitOfWork.WorkTimeRepository.SendManagerHours(employeeData.GetCurrentEmployee().Id);
                }
                else
                {
                    unitOfWork.WorkTimeRepository.SendHours(employeeData.GetCurrentEmployee().Id);
                }
                
                response.AddSuccess(Resources.WorkTimeManagement.WorkTime.SentSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            if (!response.HasErrors() && !isManager)
            {
                SendMails(response);
            }

            return response;
        }

        private void SendMails(Response response)
        {
            var employee = employeeData.GetCurrentEmployee();

            var settingCloseMonth = unitOfWork.SettingRepository.GetByKey(SettingConstant.CloseMonthKey);

            var now = DateTime.Now.Date;
            var closeMonthValue = Convert.ToInt32(settingCloseMonth.Value);

            DateTime dateFrom;
            DateTime dateTo;

            if (now.Day > closeMonthValue)
            {
                dateFrom = new DateTime(now.Year, now.Month, 1);
                dateTo = new DateTime(now.Year, now.Month + 1, 1);
            }
            else
            {
                dateFrom = new DateTime(now.Year, now.Month - 1, 1);
                dateTo = new DateTime(now.Year, now.Month, 1);
            }

            try
            {
                var managers = unitOfWork.AllocationRepository.GetManagers(employee.Id, dateFrom, dateTo);

                if (!managers.Any()) return;

                var mails = managers.Select(x => x.Email).ToList();

                foreach (var manager in managers)
                {
                    var delegates = unitOfWork.UserApproverRepository.GetByUserId(manager.Id, UserApproverType.WorkTime);

                    mails.AddRange(delegates.Select(x => x.Email));
                }

                mails = mails.Distinct().ToList();

                var subject = string.Format(Resources.Mails.MailSubjectResource.WorkTimeSendHours);

                var body = string.Format(Resources.Mails.MailMessageResource.WorkTimeSendHours);

                var recipients = string.Join(";", mails);

                var data = new MailDefaultData
                {
                    Title = subject,
                    Message = body,
                    Recipients = recipients
                };

                var email = mailBuilder.GetEmail(data);

                mailSender.Send(email);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddWarning(Resources.Common.ErrorSendMail);
            }
        }

        public Response<IList<WorkTimeReportModel>> CreateReport(ReportParams parameters)
        {
            var response = new Response<IList<WorkTimeReportModel>>();

            if (parameters.Year == 0 || parameters.Month == 0)
            {
                response.AddError(Resources.WorkTimeManagement.WorkTime.YearAndMonthRequired);
                return response;
            }

            var closeMonthSetting = unitOfWork.SettingRepository.GetByKey(SettingConstant.CloseMonthKey);
            var value = Convert.ToInt32(closeMonthSetting.Value);

            var allocations = unitOfWork.AllocationRepository.GetAllocationsForWorktimeReport(parameters);

            response.Data = new List<WorkTimeReportModel>();

            var startDate = new DateTime(parameters.Year, parameters.Month-1, value+1);
            var endDate = new DateTime(parameters.Year, parameters.Month, value);

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
                model.MonthYear = $"{allocation.StartDate.Month}-{allocation.StartDate.Year}";
                model.Facturability = allocation.Employee.BillingPercentage;
                model.AllocationPercentage = allocation.Percentage;
                model.HoursMustLoad = CalculateHoursToLoad(allocation);
                model.HoursLoaded = unitOfWork.WorkTimeRepository.GetTotalHoursBetweenDays(allocation.EmployeeId, startDate, endDate, allocation.AnalyticId);

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

            var worktime = unitOfWork.WorkTimeRepository.GetSingle(x => x.Id == id);
             
            WorkTimeValidationHandler.ValidateDelete(worktime, response, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.WorkTimeRepository.Delete(worktime);
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

            workTimeFileManager.Import(analyticId, memoryStream, response);
        }

        public byte[] ExportTemplate()
        {
            var bytes = File.ReadAllBytes($"{hostingEnvironment.ContentRootPath}/wwwroot/excelTemplates/worktime-template.xlsx");

            return bytes;
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

