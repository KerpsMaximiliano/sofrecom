using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    /// <summary>
    /// Estado para enviar a la DAF
    /// </summary>
    public class SolfacStatusInvoicePendingHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        public SolfacStatusInvoicePendingHandler(IUnitOfWork unitOfWork, ICrmInvoiceService crmInvoiceService, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
        }

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.PendingByManagementControl)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }
            
            return response;
        }

        private string GetBodyMail(Model.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusInvoicePendingMessage, link);
        }

        private string GetSubjectMail(Model.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusInvoicePendingTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipients(EmailConfig emailConfig)
        {
            return unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.InvoicePendingSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.ToBeBilled;
        }

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);
        }

        public void UpdateHitos(ICollection<string> hitos, Model.Models.Billing.Solfac solfac, string url)
        {
            crmInvoiceService.UpdateHitosStatus(hitos.ToList(), GetHitoStatus());
        }

        public void SendMail(IMailSender mailSender, Model.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subject = GetSubjectMail(solfac);
            var body = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipients = GetRecipients(emailConfig);

            var data = new SolfacStatusData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
