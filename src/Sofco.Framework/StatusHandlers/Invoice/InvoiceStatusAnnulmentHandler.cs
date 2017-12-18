using Sofco.Core.StatusHandlers;
using System;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusAnnulmentHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceStatusAnnulmentHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
            return Resources.Billing.Invoice.Cancelled;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Cancelled };
            unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
        }

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            return new Response();
        }
    }
}
