using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusRejectHandler : IInvoiceStatusHandler
    {
        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimado, </br></br>" +
                                                "El REMITO del asunto ha sido RECHAZADO por la DAF, por favor ingresar en el siguiente " +
                                                "<a href='{0}' target='_blank'>link</a> para modificar el formulario y enviar nuevamente. </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "REMITO RECHAZADO por DAF - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Invoice invoice)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.Approved || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related || invoice.InvoiceStatus == InvoiceStatus.SendPending)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.CannotReject, MessageType.Error));
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
    }
}
