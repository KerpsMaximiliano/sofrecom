using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeNewsService : IEmployeeNewsService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeNewsService> logger;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IMapper mapper;
        private readonly IMailBuilder mailBuilder;
        private readonly ISessionManager sessionManager;

        public EmployeeNewsService(IUnitOfWork unitOfWork, 
            ILogMailer<EmployeeNewsService> logger, 
            IMailSender mailSender, 
            IOptions<EmailConfig> emailOptions,
            IMapper mapper, IMailBuilder mailBuilder, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            emailConfig = emailOptions.Value;
            this.mapper = mapper;
            this.mailBuilder = mailBuilder;
            this.sessionManager = sessionManager;
        }

        public Response<IList<EmployeeNewsModel>> GetEmployeeNews()
        {
            var employeeSyncActions = unitOfWork.EmployeeSyncActionRepository.GetAll();

            var data = Translate(employeeSyncActions.ToList());

            MatchReincorporation(data);

            FillEndReason(employeeSyncActions, data);

            var response = new Response<IList<EmployeeNewsModel>> { Data = data };

            return response;
        }

        private void FillEndReason(IList<EmployeeSyncAction> employeeSyncActions, List<EmployeeNewsModel> data)
        {
            var employeesInGaps = unitOfWork.EmployeeRepository.GetByEmployeeNumbers(employeeSyncActions.Select(x => x.EmployeeNumber));

            foreach (var employeeNewsModel in data.Where(x => x.Status == "Delete"))
            {
                employeeNewsModel.EndReason = employeesInGaps
                    .SingleOrDefault(x => x.EmployeeNumber.Equals(employeeNewsModel.EmployeeNumber))
                    ?.EndReason;
            }
        }

        private void MatchReincorporation(List<EmployeeNewsModel> data)
        {
            var employeeNumbers = data
                .Where(s => s.Status == EmployeeSyncActionStatus.New)
                .Select(s => s.EmployeeNumber).ToArray();

            var storedNewEmployee = unitOfWork.EmployeeRepository.GetByEmployeeNumber(employeeNumbers);

            foreach (var employee in storedNewEmployee)
            {
                var employeeNews = data.First(s => s.EmployeeNumber == employee.EmployeeNumber);

                employeeNews.IsReincorporation = true;
            }
        }

        public Response<EmployeeSyncAction> Add(int newsId)
        {
            var userName = sessionManager.GetUserName();

            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);
            EmployeeSyncActionValidationHelper.ValidateNewStatus(response);

            if (response.HasErrors()) return response;

            try
            {
                var employee = JsonConvert.DeserializeObject<Employee>(response.Data.EmployeeData);
                employee.Created = DateTime.UtcNow;
                employee.Modified = DateTime.UtcNow;
                employee.CreatedByUser = userName;
                employee.BusinessHours = 8;
                employee.BusinessHoursDescription = "09:00 hs a 18:00 hs";

                SetEmployeeHistory(employee);

                // Add new employee
                unitOfWork.EmployeeRepository.Insert(employee);

                // Delete news
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);

                // Save all changes
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.Added);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void SetEmployeeHistory(Employee employee)
        {
            var storedEmployee =
                unitOfWork.EmployeeRepository.GetSingle(s => s.EmployeeNumber == employee.EmployeeNumber);

            if (storedEmployee == null) return;

            var employeeHistory = Translate(storedEmployee);

            unitOfWork.EmployeeHistoryRepository.Save(employeeHistory);

            unitOfWork.EmployeeRepository.Delete(storedEmployee);
        }

        public Response<EmployeeSyncAction> Delete(int newsId, NewsDeleteModel model)
        {
            var userName = sessionManager.GetUserName();

            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);
            EmployeeSyncActionValidationHelper.ValidateDeleteStatus(response);
            EmployeeSyncActionValidationHelper.ValidateEndReasonType(response, model);

            var employeeToChange = unitOfWork.EmployeeRepository.GetByEmployeeNumber(response.Data.EmployeeNumber);

            if (employeeToChange == null)
            {
                response.AddError(Resources.AllocationManagement.Employee.NotFound);
            }

            if (response.HasErrors()) return response;

            try
            {
                employeeToChange.CreatedByUser = userName;
                employeeToChange.Modified = DateTime.UtcNow;
                employeeToChange.EndDate = response.Data.EndDate;
                employeeToChange.EndReason = model.Comments;
                employeeToChange.TypeEndReasonId = model.Type.GetValueOrDefault();

                unitOfWork.EmployeeRepository.UpdateEndDate(employeeToChange);

                // Delete news
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);

                // Save all changes
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Employee.Deleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            SendMailForUnsubscribe(response, employeeToChange);

            return response;
        }

        public Response<EmployeeSyncAction> Cancel(int newsId)
        {
            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.EmployeeSyncActionRepository.Delete(response.Data);
                unitOfWork.Save();

                response.Data = null;
                response.AddSuccess(Resources.AllocationManagement.EmployeeSyncAction.Deleted);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        private void SendMailForUnsubscribe(Response<EmployeeSyncAction> response, Employee employeeToChange)
        {
            try
            {
                var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);

                var analitycs = unitOfWork.AnalyticRepository.GetAnalyticsByEmployee(employeeToChange.Id);

                var mails = new List<string> { mailPmo };
                mails.AddRange(analitycs.Select(x => x.Manager.Email).Distinct());

                var recipients = string.Join(";", mails.Distinct());

                var data = new EmployeeEndConfirmationData
                {
                    Recipients = recipients,
                    EmployeeNumber = employeeToChange.EmployeeNumber,
                    Name = employeeToChange.Name,
                    EndDate = employeeToChange.EndDate.GetValueOrDefault().ToString("d")
                };

                var mail = mailBuilder.GetEmail(data);

                mailSender.Send(mail);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSendMail);
            }
        }

        private List<EmployeeNewsModel> Translate(List<EmployeeSyncAction> employeeSyncActions)
        {
            return mapper.Map<List<EmployeeSyncAction>, List<EmployeeNewsModel>>(employeeSyncActions);
        }

        private EmployeeHistory Translate(Employee employee)
        {
            return mapper.Map<Employee, EmployeeHistory>(employee);
        }
    }
}
