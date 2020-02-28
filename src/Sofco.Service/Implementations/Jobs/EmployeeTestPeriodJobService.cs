using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Framework.MailData;
using Sofco.Resources.Mails;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeTestPeriodJobService : IEmployeeTestPeriodJobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<EmployeeTestPeriodJobService> logger;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;

        private const string Link = "https://intranet.sofrecom.com.ar/rrhh/Lists/EvaluacionIngresantes/AllItems.aspx";

        public EmployeeTestPeriodJobService(IUnitOfWork unitOfWork,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions,
            ILogMailer<EmployeeTestPeriodJobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.emailConfig = emailConfigOptions.Value;
        }

        public void Execute()
        {
            var employees = unitOfWork.EmployeeRepository.GetOnTestPeriod(DateTime.UtcNow.Date);

            if (employees.Any())
            {
                try
                {
                    var recruitersGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RRhhAb);

                    var subject = MailSubjectResource.EmployeeTestPeriod;

                    var recipients = new List<string> { recruitersGroup.Email };

                    var body = string.Format(MailMessageResource.EmployeeTestPeriod, BuildMailBody(employees, recipients), Link);

                    var maildata = new MailDefaultData
                    {
                        Title = subject,
                        Message = body,
                        Recipients = recipients
                    };

                    mailSender.Send(maildata);

                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                }
            }
        }

        private string BuildMailBody(IList<Employee> employees, List<string> recipients)
        {
            var text = new StringBuilder();

            foreach (var item in employees)
            {
                text.AppendFormat("{0} - {1} / Superior: {2}",
                    item.EmployeeNumber,
                    item.Name,
                    item.Manager?.Name);

                if (item.Manager != null)
                {
                    if (!string.IsNullOrWhiteSpace(item.Manager.Email) && !recipients.Contains(item.Manager.Email))
                    {
                        recipients.Add(item.Manager.Email);
                    }
                }

                item.OnTestPeriod = false;
                unitOfWork.EmployeeRepository.UpdateOnTestPeriod(item);
            }

            return text.ToString();
        }
    }
}
