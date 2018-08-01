using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Domain.DTO;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusSentHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailBuilder mailBuilder;

        public InvoiceStatusSentHandler(IUnitOfWork unitOfWork, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
        }

        private string mailBody = Resources.Mails.MailMessageResource.InvoiceStatusSentMessage;

        public Response Validate(Domain.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.Approved || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotSendToDaf, MessageType.Error));
            }

            if ((invoice.InvoiceStatus == InvoiceStatus.SendPending ||
                 invoice.InvoiceStatus == InvoiceStatus.Rejected) &&
                 (!invoice.ExcelFileId.HasValue || invoice.ExcelFileId.GetValueOrDefault() == 0))
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NeedExcelToSend, MessageType.Error));
            }

            if (!response.HasErrors())
            {
                if (string.IsNullOrWhiteSpace(parameters.Comment))
                    mailBody = mailBody.Replace("*", string.Empty);
                else
                    mailBody = mailBody.Replace("*", $"Comentarios: {parameters.Comment}. </br>");
            }

            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Invoice invoice, string siteUrl)
        {
            var link = $"{siteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}";

            return string.Format(mailBody, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Invoice invoice)
        {
            return string.Format(Resources.Mails.MailSubjectResource.InvoiceStatusSentTitle, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.SentToDaf;
        }

        public string GetRecipients(Domain.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            return unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
        }

        public void SaveStatus(Domain.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Domain.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Sent };
            unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
        }

        public void SendMail(IMailSender mailSender, Domain.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            var subjectToDaf = GetSubjectMail(invoice);
            var bodyToDaf = GetBodyMail(invoice, emailConfig.SiteUrl);
            var recipientsToDaf = GetRecipients(invoice, emailConfig);

            var data = new SolfacStatusData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = recipientsToDaf
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);

            var subject = string.Format(Resources.Mails.MailSubjectResource.InvoiceStatusSentTitleToUser, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
            var body = string.Format(Resources.Mails.MailMessageResource.InvoiceStatusSentMessageToUser, $"{emailConfig.SiteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}");
            var recipients = invoice.User.Email;

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
