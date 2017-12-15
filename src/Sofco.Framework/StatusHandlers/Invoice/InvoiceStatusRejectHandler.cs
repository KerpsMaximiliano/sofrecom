using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusRejectHandler : IInvoiceStatusHandler
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceStatusRejectHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        private string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimado, </br></br>" +
                                                "El REMITO del asunto ha sido RECHAZADO por la DAF, por el siguiente motivo: </br>" +
                                                "*" +
                                                "</br>" +
                                                "Por favor ingresar en el siguiente <a href='{0}' target='_blank'>link</a> para modificar el formulario y enviar nuevamente. </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "REMITO RECHAZADO por DAF - {0} - {1} - {2} - {3}";

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
                MailBody = MailBody.Replace("*", parameters.Comment);
            }

            return response;
        }

        public string GetBodyMail(Model.Models.Billing.Invoice invoice, string siteUrl)
        {
            var link = $"{siteUrl}billing/invoice/{invoice.Id}/project/{invoice.ProjectId}";

            return string.Format(MailBody, link);
        }

        public string GetSubjectMail(Model.Models.Billing.Invoice invoice)
        {
            return string.Format(MailSubject, invoice.AccountName, invoice.Service, invoice.Project, invoice.CreatedDate.ToString("yyyyMMdd"));
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.Reject;
        }

        public string GetRecipients(Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            return invoice.User.Email;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Rejected };
            _invoiceRepository.UpdateStatus(invoiceToModif);
        }
    }
}
