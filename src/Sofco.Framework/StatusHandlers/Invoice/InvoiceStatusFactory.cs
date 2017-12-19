using Sofco.Core.DAL;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusFactory : IInvoiceStatusFactory
    {
        private readonly IUnitOfWork unitOfWork;

        public InvoiceStatusFactory(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IInvoiceStatusHandler GetInstance(InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Sent: return new InvoiceStatusSentHandler(unitOfWork);
                case InvoiceStatus.Rejected: return new InvoiceStatusRejectHandler(unitOfWork);
                case InvoiceStatus.Approved: return new InvoiceStatusApproveHandler(unitOfWork);
                case InvoiceStatus.Cancelled: return new InvoiceStatusAnnulmentHandler(unitOfWork);
                default: return null;
            }
        }
    }
}
