using Sofco.Core.StatusHandlers;
using System;
using Sofco.Core.Config;
using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.Model.Enums;
using Sofco.Core.DAL.Billing;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusAnnulmentHandler : IInvoiceStatusHandler
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceStatusAnnulmentHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public string GetBodyMail(Model.Models.Billing.Invoice invoice, string siteUrl)
        {
            throw new NotImplementedException();
        }

        public string GetRecipients(Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
            throw new NotImplementedException();
        }

        public string GetSubjectMail(Model.Models.Billing.Invoice invoice)
        {
            throw new NotImplementedException();
        }

        public string GetSuccessMessage()
        {
            return Resources.es.Billing.Invoice.Cancelled;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Cancelled };
            _invoiceRepository.UpdateStatus(invoiceToModif);
        }

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            return new Response();
        }
    }
}
