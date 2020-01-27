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
using Sofco.Framework.MailData;

namespace Sofco.Service.Implementations.Jobs
{
    public class ApplicantInterviewNotificationAfterService : IApplicantInterviewNotificationAfterService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ApplicantInterviewNotificationAfterService> logger;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;

        public ApplicantInterviewNotificationAfterService(IUnitOfWork unitOfWork,
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions,
            ILogMailer<ApplicantInterviewNotificationAfterService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.mailSender = mailSender;
            this.emailConfig = emailConfigOptions.Value;
        }

        public void Execute()
        {
            var jobSearchApplicants = unitOfWork.JobSearchApplicantRepository.GetWithInterviewABeforeToday();

            if (jobSearchApplicants.Any())
            {
                try
                {
                    var recruitersGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RecruitersCode);

                    var subject = MailSubjectResource.ScheduledInterviewsAfter;

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

            foreach (var jobSearchApplicant in jobSearchApplicants)
            {
                if (jobSearchApplicant.RrhhInterviewDate.HasValue && jobSearchApplicant.ModifiedAt.Date < jobSearchApplicant.RrhhInterviewDate.Value.Date)
                {
                    text.Append($"<tr>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Reason?.Text}</td>" +
                                    $"<td class='td-md'>{jobSearchApplicant.RrhhInterviewDate.Value.Date:d}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Applicant?.LastName} {jobSearchApplicant.Applicant?.FirstName}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.RrhhInterviewer?.Name}</td>" +
                                $"</tr>");
                }

                if (jobSearchApplicant.TechnicalInterviewDate.HasValue && jobSearchApplicant.ModifiedAt.Date < jobSearchApplicant.TechnicalInterviewDate.Value.Date)
                {
                    text.Append($"<tr>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Reason?.Text}</td>" +
                                    $"<td class='td-md'>{jobSearchApplicant.TechnicalInterviewDate.Value.Date:d}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Applicant?.LastName} {jobSearchApplicant.Applicant?.FirstName}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.TechnicalExternalInterviewer}</td>" +
                                $"</tr>");
                }

                if (jobSearchApplicant.ClientInterviewDate.HasValue && jobSearchApplicant.ModifiedAt.Date < jobSearchApplicant.ClientInterviewDate.Value.Date)
                {
                    text.Append($"<tr>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Reason?.Text}</td>" +
                                    $"<td class='td-md'>{jobSearchApplicant.ClientInterviewDate.Value.Date:d}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.Applicant?.LastName} {jobSearchApplicant.Applicant?.FirstName}</td>" +
                                    $"<td class='td-lg'>{jobSearchApplicant.ClientExternalInterviewer}</td>" +
                                $"</tr>");
                }
            }

            return text.ToString();
        }
    }
}
