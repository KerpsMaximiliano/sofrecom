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
    public class SolfacStatusAmountCashedHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoicingMilestoneService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public SolfacStatusAmountCashedHandler(IUnitOfWork unitOfWork, ICrmInvoicingMilestoneService crmInvoiceService, IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response Validate(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            if (solfac.Status != SolfacStatus.Invoiced)
            {
                response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            }

            SolfacValidationHelper.ValidateCasheDate(parameters, response, solfac);

            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusAmountCashedMessage, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusAmountCashedTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
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
            return HitoStatus.Cashed;
        }

        public void SaveStatus(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Domain.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status, CashedDate = parameters.CashedDate };
            unitOfWork.SolfacRepository.UpdateStatusAndCashed(solfacToModif);
            solfac.CashedDate = parameters.CashedDate;
        }

        public void UpdateHitos(ICollection<string> hitos, Domain.Models.Billing.Solfac solfac)
        {
            crmInvoiceService.UpdateStatusAndBillingDate(hitos.ToList(), GetHitoStatus(), solfac.CashedDate.GetValueOrDefault());
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
