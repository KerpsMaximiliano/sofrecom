﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.DAL;
using Sofco.Model;
using Sofco.Model.Models.AllocationManagement;
using Sofco.Service.Settings.Jobs;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeEndJobService : IEmployeeEndJobService
    {
        const string DateFormat = "dd/MM/yyyy";

        const string Subject = "Empleados con baja";

        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly JobSetting setting;

        private readonly EmailConfig emailConfig;

        public EmployeeEndJobService(IUnitOfWork unitOfWork,
            IMailBuilder mailBuilder,
            IMailSender mailSender,
            IOptions<JobSetting> jobSettingOptions,
            IOptions<EmailConfig> emailConfigOptions)
        {
            this.unitOfWork = unitOfWork;

            this.mailBuilder = mailBuilder;

            this.mailSender = mailSender;

            setting = jobSettingOptions.Value;

            emailConfig = emailConfigOptions.Value;
        }

        public void SendNotification()
        {
            var localToday = TimeZoneInfo
                .ConvertTime(DateTime.Now.AddDays(setting.EmployeeEndJob.DaysFromNow),
                TimeZoneInfo.FindSystemTimeZoneById(setting.LocalTimeZoneName));

            var employeeEnds = unitOfWork.EmployeeRepository.GetByEndDate(localToday.Date);

            if (!employeeEnds.Any()) return;

            mailSender.Send(GetEmail(employeeEnds));
        }

        private Email GetEmail(List<Employee> employeeEnds)
        {
            var mailTos = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);

            var data = new Dictionary<string, string>();

            var content = new StringBuilder();

            foreach (var item in employeeEnds)
            {
                var link = $"{emailConfig.SiteUrl}";

                content.AppendLine($"<li><a href='{link}'>{item.Name} - {item.EmployeeNumber} - {item.EndDate?.ToString(DateFormat)}</a>");
            }

            data.Add("content", $"<ul>{content}</ul>");
            data.Add("title", Subject);
            data.Add("message", string.Empty);

            var mail = mailBuilder.GetEmail(MailType.Default, mailTos, Subject, data);

            return mail;
        }
    }
}
