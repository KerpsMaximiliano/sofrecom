using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusSentHandler : IInvoiceStatusHandler
    {
        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "Se ha cargado un REMITO que requiere revisión y generación. </br>" +
                                                "Para imprimirlo, utilice el documento adjunto. </br>" +
                                                "Una vez generado el pdf, por favor adjuntarlo en el siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "REMITO - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Invoice invoice)
        {
            var response = new Response();

            if (invoice.InvoiceStatus == InvoiceStatus.Approved || invoice.InvoiceStatus == InvoiceStatus.Cancelled ||
                invoice.InvoiceStatus == InvoiceStatus.Related)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.CannotSendToDaf, MessageType.Error));
            }

            if ((invoice.InvoiceStatus == InvoiceStatus.SendPending ||
                 invoice.InvoiceStatus == InvoiceStatus.Rejected) &&
                string.IsNullOrWhiteSpace(invoice.ExcelFileName))
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NeedExcelToSend, MessageType.Error));
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
