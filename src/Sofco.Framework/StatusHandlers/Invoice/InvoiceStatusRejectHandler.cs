using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.MailData;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusRejectHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailBuilder mailBuilder;

        public InvoiceStatusRejectHandler(IUnitOfWork unitOfWork, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
        }

        private string mailBody = Resources.Mails.MailMessageResource.InvoiceStatusRejectMessage;

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.Approved || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related || invoice.InvoiceStatus == InvoiceStatus.SendPending)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotReject, MessageType.Error));
            }

            if (!response.HasErrors())
            {
                mailBody = mailBody.Replace("*", parameters.Comment);
            }

            return response;
        }

        private string GetBodyMail(Model.Models.Billing.Invoice invoice, string siteUrl)
        {
            var link = $"{siteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}";

            return string.Format(mailBody, link);
        }

        private string GetSubjectMail(Model.Models.Billing.Invoice invoice)
        {
            return string.Format(Resources.Mails.MailSubjectResource.InvoiceStatusRejectTitle, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.Reject;
        }

        private string GetRecipients(Model.Models.Billing.Invoice invoice)
        {
            return invoice.User.Email;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Rejected };
            unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
        }

        public void SendMail(IMailSender mailSender, Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            var subjectToDaf = GetSubjectMail(invoice);
            var bodyToDaf = GetBodyMail(invoice, emailConfig.SiteUrl);
            var recipientsToDaf = GetRecipients(invoice);

            var data = new SolfacStatusData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipients = recipientsToDaf
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
