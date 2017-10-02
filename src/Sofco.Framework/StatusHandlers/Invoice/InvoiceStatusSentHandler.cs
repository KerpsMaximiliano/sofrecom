using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Utils;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusSentHandler : IInvoiceStatusHandler
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceStatusSentHandler(IGroupRepository groupRepository, IInvoiceRepository invoiceRepository)
        {
            _groupRepository = groupRepository;
            _invoiceRepository = invoiceRepository;
        }

        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "Se ha cargado un REMITO que requiere revisión y generación (pdf). </br>" +
                                                "Para imprimirlo, utilice el documento anexado al registro. </br>" +
                                                "Una vez generado el pdf, por favor importarlo en el siguiente <a href='{0}' target='_blank'>link</a>. </br></br>" +
                                                "Muchas gracias." +
                                            "</span>" +
                                        "</font>";

        private const string MailSubject = "REMITO - {0} - {1} - {2} - {3}";

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
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

        public string GetSuccessMessage()
        {
            return Resources.es.Billing.Invoice.SentToDaf;
        }

        public string GetRecipients(Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            var group = _groupRepository.GetSingle(x => x.Id == emailConfig.DafMail);
            return group.Email;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Sent };
            _invoiceRepository.UpdateStatus(invoiceToModif);
        }
    }
}
