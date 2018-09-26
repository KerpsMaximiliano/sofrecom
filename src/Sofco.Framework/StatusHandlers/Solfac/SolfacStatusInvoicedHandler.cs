using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusInvoicedHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public SolfacStatusInvoicedHandler(IUnitOfWork unitOfWork, ICrmInvoiceService crmInvoiceService, IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response Validate(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.InvoicePending)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateInvoiceCode(parameters, unitOfWork.SolfacRepository, response, solfac.InvoiceCode);
            SolfacValidationHelper.ValidateInvoiceDate(parameters, response, solfac);
            
            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusInvoicedMessage, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusInvoicedTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipient(Domain.Models.Billing.Solfac solfac)
        {
            return solfac.UserApplicant.Email;
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.InvoicedSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.Billed;
        }

        public void SaveStatus(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Domain.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status, InvoiceCode = parameters.InvoiceCode, InvoiceDate = parameters.InvoiceDate };
            unitOfWork.SolfacRepository.UpdateStatusAndInvoice(solfacToModif);
            solfac.InvoiceDate = parameters.InvoiceDate;
            solfac.InvoiceCode = parameters.InvoiceCode;
        }

        public void UpdateHitos(ICollection<string> hitos, Domain.Models.Billing.Solfac solfac, string url)
        {
            crmInvoiceService.UpdateHitosStatusAndInvoiceDateAndNumber(hitos.ToList(), GetHitoStatus(), solfac.InvoiceDate.GetValueOrDefault(), solfac.InvoiceCode);
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
