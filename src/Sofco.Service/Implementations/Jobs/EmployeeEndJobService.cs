using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Services.Jobs;
using Sofco.Service.Settings.Jobs;
using Sofco.Model.Models.TimeManagement;
using Sofco.Resources;
using Sofco.Core.Config;
using Sofco.Core.Mail;
using Sofco.Model;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeEndJobService : IEmployeeEndJobService
    {
        const string DateFormat = "dd/MM/yyyy";

        const string Subject = "Empleados con baja";

        private readonly IEmployeeRepository employeeRepository;

        private readonly IMailSender mailSender;

        private readonly JobSetting setting;

        private readonly EmailConfig emailConfig;

        public EmployeeEndJobService(IEmployeeRepository employeeRepository,
            IMailSender mailSender,
            IOptions<JobSetting> jobSettingOptions,
            IOptions<EmailConfig> emailConfigOptions)
        {
            this.employeeRepository = employeeRepository;

            this.mailSender = mailSender;

            setting = jobSettingOptions.Value;

            emailConfig = emailConfigOptions.Value;
        }

        public void SendNotification()
        {
            var localToday = TimeZoneInfo
                .ConvertTime(DateTime.Now.AddDays(setting.EmployeeEndJob.DaysFromNow),
                TimeZoneInfo.FindSystemTimeZoneById(setting.LocalTimeZoneName));

            var employeeEnds = employeeRepository.GetByEndDate(localToday.Date);

            if (!employeeEnds.Any()) return;

            var email = GetEmail(employeeEnds);

            mailSender.Send(email);
        }

        private Email GetEmail(List<Employee> employeeEnds)
        {
            var result = new Email
                {
                    Subject = Subject,
                    Recipient = setting.EmployeeEndJob.MailTo,
                    Body = BuildBody(employeeEnds)
                };

            return result;
        }

        private string BuildBody(List<Employee> employees)
        {
            var template = MailResource.DefaultTemplate;

            var content = new StringBuilder();

            foreach (var item in employees)
            {
                var link = $"{emailConfig.SiteUrl}";

                content.AppendLine($"<li><a href='{link}'>{item.Name} - {item.EmployeeNumber} - {item.EndDate?.ToString(DateFormat)}</a>");
            }

            var body = template.Replace("{content}", $"<ul>{content.ToString()}</ul>");

            body = body.Replace("{title}", Subject);

            body = body.Replace("{message}", "");

            body = body.Replace("{homeLink}", emailConfig.SiteUrl);

            return body;
        }
    }
}
