using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.Services.Jobs;
using Sofco.Framework.MailData;
using Sofco.Domain;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Service.Implementations.Jobs
{
    public class LicenseCertificatePendingJobService : ILicenseCertificatePendingJobService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly EmailConfig emailConfig;

        public LicenseCertificatePendingJobService(IUnitOfWork unitOfWork, 
            IMailBuilder mailBuilder, 
            IMailSender mailSender,
            IOptions<EmailConfig> emailConfigOptions)
        {
            this.unitOfWork = unitOfWork;

            this.mailBuilder = mailBuilder;

            this.mailSender = mailSender;

            emailConfig = emailConfigOptions.Value;
        }

        public void SendNotifications()
        {
            var licenses = unitOfWork.LicenseRepository.GetPendingCertificates();

            if (!licenses.Any()) return;

            mailSender.Send(GetEmail(licenses));
        }

        private Email GetEmail(List<License> licenses)
        {
            var content = new StringBuilder();

            foreach (var item in licenses)
            {
                content.AppendLine($"<li>{item.Employee.Name} - {item.Type.Description}");
            }

            var rrhhAbMail = unitOfWork.GroupRepository.GetEmail(emailConfig.RRhhAb);
            var rrhhLMail = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhL);

            return mailBuilder.GetEmail(new LicensePendingCertificateData
            {
            
                Recipient = $"{rrhhAbMail};{rrhhLMail}",
                Message = $"<ul>{content}</ul>"
            });
        }
    }
}
