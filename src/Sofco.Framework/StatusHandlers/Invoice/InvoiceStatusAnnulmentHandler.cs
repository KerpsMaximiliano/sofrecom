using Sofco.Core.Config;
using Sofco.Core.StatusHandlers;
using Sofco.Core.DAL;
using Sofco.Core.Mail;
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

        public string GetSuccessMessage()
        {
            return Resources.Billing.Invoice.Cancelled;
        }

        public void SaveStatus(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            var invoiceToModif = new Model.Models.Billing.Invoice { Id = invoice.Id, InvoiceStatus = InvoiceStatus.Cancelled };
            unitOfWork.InvoiceRepository.UpdateStatus(invoiceToModif);
        }

        public void SendMail(IMailSender mailSender, Model.Models.Billing.Invoice invoice, EmailConfig emailConfig)
        {
        }

        public Response Validate(Model.Models.Billing.Invoice invoice, InvoiceStatusParams parameters)
        {
            return new Response();
        }
    }
}
