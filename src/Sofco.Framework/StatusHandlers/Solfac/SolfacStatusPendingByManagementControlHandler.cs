using System.Collections.Generic;
using System.Linq;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.Billing;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Solfac
{
    public class SolfacStatusPendingByManagementControlHandler : ISolfacStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ICrmInvoiceService crmInvoiceService;

        private readonly IMailBuilder mailBuilder;

        public SolfacStatusPendingByManagementControlHandler(IUnitOfWork unitOfWork, ICrmInvoiceService crmInvoiceService, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.crmInvoiceService = crmInvoiceService;
            this.mailBuilder = mailBuilder;
        }

        public Response Validate(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
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

        private string GetBodyMail(Model.Models.Billing.Solfac solfac, string siteUrl)
        {
            var link = $"{siteUrl}billing/solfac/{solfac.Id}";

            return string.Format(Resources.Mails.MailMessageResource.SolfacStatusPendingByManagementControlMessage, link);
        }

        private string GetSubjectMail(Model.Models.Billing.Solfac solfac)
        {
            return string.Format(Resources.Mails.MailSubjectResource.SolfacStatusPendingByManagementControlTitle, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
        }

        private string GetRecipients(EmailConfig emailConfig)
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

        public void SaveStatus(Model.Models.Billing.Solfac solfac, SolfacStatusParams parameters)
        {
            var solfacToModif = new Model.Models.Billing.Solfac { Id = solfac.Id, Status = parameters.Status };
            unitOfWork.SolfacRepository.UpdateStatus(solfacToModif);

            if (solfac.PurchaseOrder == null) return;

            var detail = solfac.PurchaseOrder.AmmountDetails.SingleOrDefault(x => x.CurrencyId == solfac.CurrencyId);

            if (detail != null)
            {
                unitOfWork.PurchaseOrderRepository.UpdateBalance(detail);
                detail.Balance = detail.Balance - solfac.TotalAmount;
            }
        }

        public void UpdateHitos(ICollection<string> hitos, Model.Models.Billing.Solfac solfac, string url)
        {
            crmInvoiceService.UpdateHitosStatusAndPurchaseOrder(hitos.ToList(), GetHitoStatus(), solfac.PurchaseOrder.Number);
        }

        public void SendMail(IMailSender mailSender, Model.Models.Billing.Solfac solfac, EmailConfig emailConfig)
        {
            var subjectToCdg = GetSubjectMail(solfac);
            var bodyToCdg = GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipientsToCdg = GetRecipients(emailConfig);

            var data = new SolfacStatusData
            {
                Title = subjectToCdg,
                Message = bodyToCdg,
                Recipients = recipientsToCdg
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);

            var subject = string.Format(Resources.Mails.MailSubjectResource.SolfacStatusPendingByManagementControlTitleToUser, solfac.BusinessName, solfac.Service, solfac.Project, solfac.StartDate.ToString("yyyyMMdd"));
            var body = string.Format(Resources.Mails.MailMessageResource.SolfacStatusPendingByManagementControlMessageToUser, $"{emailConfig.SiteUrl}billing/solfac/{solfac.Id}");
            var recipients = solfac.UserApplicant.Email;

            data = new SolfacStatusData
            {
                Title = subject,
                Message = body,
                Recipients = recipients
            };

            email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
