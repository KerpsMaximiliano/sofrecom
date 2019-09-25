using Sofco.Common.Security.Interfaces;
using Sofco.Core.Data.Billing;
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
        private readonly IInvoiceData invoiceData;
        private readonly ISessionManager sessionManager;

        public InvoiceStatusFactory(IUnitOfWork unitOfWork, IMailBuilder mailBuilder, IInvoiceData invoiceData, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.mailBuilder = mailBuilder;
            this.invoiceData = invoiceData;
            this.sessionManager = sessionManager;
        }

        public IInvoiceStatusHandler GetInstance(InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Sent: return new InvoiceStatusSentHandler(unitOfWork, mailBuilder, invoiceData, sessionManager);
                case InvoiceStatus.Rejected: return new InvoiceStatusRejectHandler(unitOfWork, mailBuilder);
                case InvoiceStatus.Approved: return new InvoiceStatusApproveHandler(unitOfWork, mailBuilder);
                case InvoiceStatus.Cancelled: return new InvoiceStatusAnnulmentHandler(unitOfWork);
                default: return null;
            }
        }
    }
}
