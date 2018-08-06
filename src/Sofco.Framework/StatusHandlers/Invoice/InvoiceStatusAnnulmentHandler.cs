using Sofco.Core.Config;
using Sofco.Core.StatusHandlers;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using Sofco.Domain.Enums;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusAnnulmentHandler : IInvoiceStatusHandler
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceStatusAnnulmentHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.Cancelled;
        }

        public void SaveStatus(Domain.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Domain.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Cancelled };
            unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
        }

        public void SendMail(IMailSender mailSender, Domain.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
        }

        public Response Validate(Domain.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            return new Response();
        }
    }
}
