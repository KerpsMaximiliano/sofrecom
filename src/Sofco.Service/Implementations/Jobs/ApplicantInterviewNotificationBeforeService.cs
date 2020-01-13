using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Models.Recruitment;
using Sofco.Resources.Mails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sofco.Domain.Enums;
using Sofco.Framework.MailData;

namespace Sofco.Service.Implementations.Jobs
{
    public class ApplicantInterviewNotificationBeforeService : IApplicantInterviewNotificationBeforeService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ApplicantInterviewNotificationBeforeService> logger;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;

        public ApplicantInterviewNotificationBeforeService(IUnitOfWork unitOfWork,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions,
            ILogMailer<ApplicantInterviewNotificationBeforeService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.emailConfig = emailConfigOptions.Value;
        }

        public void Execute()
        {
            var jobSearchApplicants = unitOfWork.JobSearchApplicantRepository.GetWithInterviewAfterToday();

            if (jobSearchApplicants.Any())
            {
                try
                {
                    var recruitersGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RecruitersCode);

                    var subject = string.Format(MailSubjectResource.ScheduledInterviews, DateTime.UtcNow.AddDays(1).Date.ToString("d"));

                    var body = string.Format(MailMessageResource.ScheduledInterviews, BuildMailBody(jobSearchApplicants));

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

        private string BuildMailBody(IList<JobSearchApplicant> jobSearchApplicants)
        {
            var text = new StringBuilder();
            var today = DateTime.UtcNow.Date;

            foreach (var jobSearchApplicant in jobSearchApplicants)
            {
                if (jobSearchApplicant.RrhhInterviewDate.HasValue &&
                    jobSearchApplicant.RrhhInterviewDate.Value.Date.AddDays(-1) == today.Date)
                {
                    text.Append($"<tr>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Reason?.Text}</td>" +
                                    $"<td class='td-md'>{today.AddDays(1).Date:d}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Applicant?.LastName} {jobSearchApplicant.Applicant?.FirstName}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.RrhhInterviewer?.Name}</td>" +
                                $"</tr>");
                }

                if (jobSearchApplicant.TechnicalInterviewDate.HasValue &&
                    jobSearchApplicant.TechnicalInterviewDate.Value.Date.AddDays(-1) == today.Date)
                {
                    text.Append($"<tr>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Reason?.Text}</td>" +
                                    $"<td class='td-md'>{today.AddDays(1).Date:d}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Applicant?.LastName} {jobSearchApplicant.Applicant?.FirstName}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.TechnicalExternalInterviewer}</td>" +
                                $"</tr>");
                }

                if (jobSearchApplicant.ClientInterviewDate.HasValue &&
                    jobSearchApplicant.ClientInterviewDate.Value.Date.AddDays(-1) == today.Date)
                {
                    text.Append($"<tr>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Reason?.Text}</td>" +
                                    $"<td class='td-md'>{today.AddDays(1).Date:d}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Applicant?.LastName} {jobSearchApplicant.Applicant?.FirstName}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.ClientInterviewDate}</td>" +
                                $"</tr>");
                }
            }

            return text.ToString();
        }
    }
}
