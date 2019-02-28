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
    public class SolfacStatusPendingByManagementControlHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoicingMilestoneService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        private readonly IMailSender mailSender;

        public SolfacStatusPendingByManagementControlHandler(IUnitOfWork unitOfWork, ICrmInvoicingMilestoneService crmInvoiceService, 
                                                             IMailBuilder mailBuilder, IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
            this.mailSender = mailSender;
        }

        public Response Validate(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var response = new Response();

            SolfacValidationHelper.ValidateDetails(solfac.Hitos, response);

            if (solfac.Status == SolfacStatus.SendPending || solfac.Status == SolfacStatus.ManagementControlRejected || solfac.Status == SolfacStatus.RejectedByDaf)
            {
                if (solfac.InvoiceRequired && solfac.DocumentTypeId != SolfacDocumentType.CreditNoteA 
                                           && solfac.DocumentTypeId != SolfacDocumentType.CreditNoteB
                                           && solfac.DocumentTypeId != SolfacDocumentType.DebitNote
                                           && !unitOfWork.SolfacRepository.HasInvoices(solfac.Id))
                {
                    response.AddWarning(Resources.Billing.Solfac.SolfacHasNoInvoices);
                }

                if (!response.HasErrors() && (!unitOfWork.SolfacRepository.HasAttachments(solfac.Id) && !unitOfWork.SolfacCertificateRepository.HasCertificates(solfac.Id)))
                {
                    response.AddWarning(Resources.Billing.Solfac.SolfacHasNoAttachments);
                }

                return response;
            }

            response.Messages.Add(new Message(Resources.Billing.Solfac.CannotChangeStatus, MessageType.Error));
            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusPendingByManagementControlMessage, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusPendingByManagementControlTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipient(EmailConfig emailConfig)
        {
            return unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Solfac.PendingByManagementControlSuccess;
        }

        private HitoStatus GetHitoStatus()
        {
            return HitoStatus.Pending;
        }

        public void SaveStatus(Domain.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Domain.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);
        }

        public void UpdateHitos(ICollection<string> hitos, Domain.Models.Billing.Solfac solfac)
        {
            string ocNumber;

            if (solfac.PurchaseOrder == null)
            {
                var oc = unitOfWork.PurchaseOrderRepository.Get(solfac.PurchaseOrderId.GetValueOrDefault());
                ocNumber = oc.Number;
            }
            else
            {
                ocNumber = solfac.PurchaseOrder.Number;
            }

            crmInvoiceService.UpdateStatusAndPurchaseOrder(hitos.ToList(), GetHitoStatus(), ocNumber);
        }


        public void SendMail(Domain.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subjectToCdg = GetSubjectMail(solfac);
            var bodyToCdg = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipientsToCdg = GetRecipient(emailConfig);

            var data = new SolfacStatusData
            {
                Title = subjectToCdg,
                Message = bodyToCdg,
                Recipient = recipientsToCdg
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);

            var subject = string.Format(Resources.Mails.MailSubjectResource.SolfacStatusPendingByManagementControlTitleToUser, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
            var body = string.Format(Resources.Mails.MailMessageResource.SolfacStatusPendingByManagementControlMessageToUser, $"{emailConfig.SiteUrl}billing/solfac/{solfac.Id}");
            var recipient = solfac.UserApplicant.Email;

            data = new SolfacStatusData
            {
                Title = subject,
                Message = body,
                Recipient = recipient
            };

            email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
