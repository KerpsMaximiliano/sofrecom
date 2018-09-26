using System;
using System.Collections.Generic;
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
    public class InvoiceStatusApproveHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        public InvoiceStatusApproveHandler(IUnitOfWork unitOfWork, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
        }

        public Response Validate(Domain.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.SendPending || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related || invoice.InvoiceStatus == InvoiceStatus.Rejected)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotApprove, MessageType.Error));
            }

            if (invoice.InvoiceStatus == InvoiceStatus.Sent && (!invoice.PdfFileId.HasValue || invoice.PdfFileId.GetValueOrDefault() == 0))
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NeedPdfToApprove, MessageType.Error));
            }

            if (string.IsNullOrWhiteSpace(parameters.InvoiceNumber))
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.InvoiceNumerRequired, MessageType.Error));
            }
            else
            {
                if (unitOfWork.InvoiceRepository.InvoiceNumberExist(parameters.InvoiceNumber))
                {
                    response.Messages.Add(new Message(Resources.Billing.Invoice.InvoiceNumerAlreadyExist, MessageType.Error));
                }
            }

            return response;
        }

        private string GetBodyMail(Domain.Models.Billing.Invoice invoice, string siteUrl)
        {
            var link = $"{siteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}";

            return string.Format(Resources.Mails.MailMessageResource.InvoiceStatusApproveMessage, link);
        }

        private string GetSubjectMail(Domain.Models.Billing.Invoice invoice)
        {
            return string.Format(Resources.Mails.MailSubjectResource.InvoiceStatusApproveTitle, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.Approved;
        }

        private string GetRecipients(Domain.Models.Billing.Invoice invoice)
        {
            return invoice.User.Email;
        }

        public void SaveStatus(Domain.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Domain.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Approved, InvoiceNumber = parameters.InvoiceNumber };

            unitOfWork.InvoiceRepository.UpdateStatusAndApprove(invoiceToModif);
        }

        public void SendMail(IMailSender mailSender, Domain.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            var subjectToDaf = GetSubjectMail(invoice);
            var bodyToDaf = GetBodyMail(invoice, emailConfig.SiteUrl);
            var recipientsToDaf = GetRecipients(invoice);

            var data = new SolfacStatusData
            {
                Title = subjectToDaf,
                Message = bodyToDaf,
                Recipient = recipientsToDaf
            };

            var email = mailBuilder.GetEmail(data);
            mailSender.Send(email);
        }
    }
}
