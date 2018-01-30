using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
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

        private const string MailSubject = "BAJA DE RECURSO";

        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "RRHH ha confirmado la baja del siguiente recurso: </br></br>" +
                                                "Legajo: {0}</br>" +
                                                "Nombre: {1}</br>" +
                                                "Fecha de baja: {2} </br></br>" +
                                                "Muchas Gracias" +
                                            "</span>" +
                                        "</font>";

        public EmployeeNewsService(IUnitOfWork unitOfWork, 
            ILogMailer<EmployeeNewsService> logger, 
            IMailSender mailSender, 
            IOptions<EmailConfig> emailOptions,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            emailConfig = emailOptions.Value;
            this.mapper = mapper;
        }

        public Response<IList<EmployeeNewsModel>> GetEmployeeNews()
        {
            var employeeSyncActions = unitOfWork.EmployeeSyncActionRepository.GetAll();

            var data = Translate(employeeSyncActions.ToList());

            var response = new Response<IList<EmployeeNewsModel>> { Data = data };

            return response;
        }

        public Response<EmployeeSyncAction> Add(int newsId, string userName)
        {
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
                this.logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<EmployeeSyncAction> Delete(int newsId, string userName)
        {
            var response = new Response<EmployeeSyncAction>();

            EmployeeSyncActionValidationHelper.Exist(newsId, response, unitOfWork);
            EmployeeSyncActionValidationHelper.ValidateDeleteStatus(response);

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

        private void SendMailForUnsubscribe(Response<EmployeeSyncAction> response, Employee employeeToChange)
        {
            try
            {
                var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);

                var analitycs = unitOfWork.AnalyticRepository.GetAnalyticsByEmployee(employeeToChange.Id);

                var mails = new List<string> { mailPmo };
                mails.AddRange(analitycs.Select(x => x.Manager.Email).Distinct());

                var recipients = string.Join(";", mails.Distinct());

                var body = string.Format(MailBody, employeeToChange.EmployeeNumber, employeeToChange.Name, employeeToChange.EndDate.GetValueOrDefault().ToString("d"));

                mailSender.Send(recipients, MailSubject, body);
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
    }
}
