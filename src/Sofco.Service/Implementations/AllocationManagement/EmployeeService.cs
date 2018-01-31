using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Framework.MailData;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Resources.Mails;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeService> logger;
        private readonly IMailSender mailSender;
        private readonly IMailBuilder mailBuilder;
        private readonly EmailConfig emailConfig;

        public EmployeeService(IUnitOfWork unitOfWork, ILogMailer<EmployeeService> logger, IMailSender mailSender, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.mailBuilder = mailBuilder;
            emailConfig = emailOptions.Value;
        }

        public ICollection<Employee> GetAll()
        {
            return unitOfWork.EmployeeRepository.GetAll();
        }

        public Response<Employee> GetById(int id)
        {
            var response = new Response<Employee>();

            response.Data = EmployeeValidationHelper.Find(response, unitOfWork.EmployeeRepository, id);

            return response;
        }

        public ICollection<Employee> Search(EmployeeSearchParams parameters)
        {
            return unitOfWork.EmployeeRepository.Search(parameters);
        }

        public Response SendUnsubscribeNotification(string employeeName, UnsubscribeNotificationParams parameters)
        {
            var response = new Response();

            if (string.IsNullOrWhiteSpace(employeeName))
            {
                response.AddError(Resources.AllocationManagement.Employee.NameRequired);
                return response;
            }

            var manager = unitOfWork.UserRepository.GetSingle(x => x.UserName.Equals(parameters.UserName));

            var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);

            parameters.Receipents.Add(mailRrhh);
            
            try
            {
                var body = string.Format(MailMessageResource.EmployeeEndNotification, employeeName, manager.Name, parameters.EndDate.ToString("d"));

                var data = new DefaultMailData
                {
                    Title = MailSubjectResource.EmployeeEndNotification,
                    Recipients = string.Join(";", parameters.Receipents),
                    Message = body
                };

                var email = mailBuilder.GetEmail(data);

                mailSender.Send(email);

                response.AddSuccess(Resources.Common.MailSent);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSendMail);
            }

            return response;
        }
    }
}
