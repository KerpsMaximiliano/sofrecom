using Sofco.Core.DAL;
using Sofco.Core.Mail;
using Sofco.Core.StatusHandlers;
using Sofco.Domain.Enums;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusFactory : IInvoiceStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly IMailBuilder mailBuilder;

        public InvoiceStatusFactory(IUnitOfWork unitOfWork, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
        }

        public IInvoiceStatusHandler GetInstance(InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Sent: return new InvoiceStatusSentHandler(unitOfWork, mailBuilder);
                case InvoiceStatus.Rejected: return new InvoiceStatusRejectHandler(unitOfWork, mailBuilder);
                case InvoiceStatus.Approved: return new InvoiceStatusApproveHandler(unitOfWork, mailBuilder);
                case InvoiceStatus.Cancelled: return new InvoiceStatusAnnulmentHandler(unitOfWork);
                default: return null;
            }
        }
    }
}
