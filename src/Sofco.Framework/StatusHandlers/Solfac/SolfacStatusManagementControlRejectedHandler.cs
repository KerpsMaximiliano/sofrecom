using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Service.Crm.Interfaces;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusRejectedByDafHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        private readonly ICrmInvoicingMilestoneService crmInvoiceService;

        public SolfacStatusRejectedByDafHandler(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IMailSender mailSender, ICrmInvoicingMilestoneService crmInvoiceService)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
            this.crmInvoiceService = crmInvoiceService;
        }

        private string mailBody = Resources.Mails.MailMessageResource.SolfacStatusRejectedByDafMessage;

        public Response Validate(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.InvoicePending)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateComments(parameters, response);

            if (!response.HasErrors())
            {
                mailBody = mailBody.Replace("*", parameters.Comment);
            }
            
            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(mailBody, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusRejectedByDafTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipient(Domain.Models.Billing.Solfac solfac)
        {
            return solfac.UserApplicant.Email;
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.DafRejectedSuccess;
        }

        public void SaveStatus(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Domain.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);
        }

        public void UpdateHitos(ICollection<string> hitos, Domain.Models.Billing.Solfac solfac)
        {
            crmInvoiceService.UpdateStatus(hitos.ToList(), HitoStatus.Pending);
        }

        public void SendMail(Domain.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subject = GetSubjectMail(solfac);
            var body = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipient = GetRecipient(solfac);

            var data = new SolfacStatusData
            {
                Title = subject,
                Message = body,
                Recipient = recipient
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
