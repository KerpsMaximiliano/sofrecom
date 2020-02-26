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
    public class LicenseGreater35JobService : ILicenseGreater35JobService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<LicenseGreater35JobService> logger;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;

        public LicenseGreater35JobService(IUnitOfWork unitOfWork,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions,
            ILogMailer<LicenseGreater35JobService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.emailConfig = emailConfigOptions.Value;
        }

        public void Execute()
        {
            var employees = unitOfWork.EmployeeRepository.GetWithHolidaysPendingGreaterThen35();

            if (employees.Any())
            {
                try
                {
                    var recruitersGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RrhhL);

                    var subject = MailSubjectResource.LicenseGreater35Title;

                    var body = string.Format(MailMessageResource.LicenseGreater35Body, BuildMailBody(employees));

                    var recipients = new List<string> { recruitersGroup.Email };

                    var maildata = new MailDefaultData
                    {
                        Title = subject,
                        Message = body,
                        Recipients = recipients
                    };

                    mailSender.Send(maildata);
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                }
            }
        }

        private string BuildMailBody(IList<Employee> employees)
        {
            var text = new StringBuilder();

            foreach (var employee in employees)
            {
                text.Append($"<tr>" +
                                $"<td class='td-md'>{employee.EmployeeNumber}</td>" +
                                $"<td class='td-lg'>{employee.Name}</td>" +
                                $"<td class='td-md'>{employee.HolidaysPending} días</td>" +
                            $"</tr>");
            }

            return text.ToString();
        }
    }
}
