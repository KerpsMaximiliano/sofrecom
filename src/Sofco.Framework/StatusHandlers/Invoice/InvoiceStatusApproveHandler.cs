using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusApproveHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceStatusApproveHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private string mailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimado, </br></br>" +
                                                "El REMITO del asunto se encuentra GENERADO. Para acceder, por favor " +
                                                "ingresar al siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                                "Muchas gracias" +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "REMITO GENERADO - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.SendPending || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related || invoice.InvoiceStatus == InvoiceStatus.Rejected)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotApprove, MessageType.Error));
            }

            if (invoice.InvoiceStatus == InvoiceStatus.Sent && string.IsNullOrWhiteSpace(invoice.PdfFileName))
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

        public string GetBodyMail(Model.Models.Billing.Invoice invoice, string siteUrl)
        {
            var link = $"{siteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}";

            return string.Format(mailBody, link);
        }

        public string GetSubjectMail(Model.Models.Billing.Invoice invoice)
        {
            return string.Format(MailSubject, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.Approved;
        }

        public string GetRecipients(Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            return invoice.User.Email;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Approved, InvoiceNumber = parameters.InvoiceNumber };
            unitOfWork.InvoiceRepository.UpdateStatusAndApprove(invoiceToModif);
        }
    }
}
