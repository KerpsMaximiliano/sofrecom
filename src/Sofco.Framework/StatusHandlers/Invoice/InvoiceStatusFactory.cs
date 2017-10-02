using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public class InvoiceStatusFactory : IInvoiceStatusFactory
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoiceStatusFactory(IGroupRepository groupRepository, IInvoiceRepository invoiceRepository)
        {
            _groupRepository = groupRepository;
            _invoiceRepository = invoiceRepository;
        }

        public IInvoiceStatusHandler GetInstance(InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Sent: return new InvoiceStatusSentHandler(_groupRepository, _invoiceRepository);
                case InvoiceStatus.Rejected: return new InvoiceStatusRejectHandler(_invoiceRepository);
                case InvoiceStatus.Approved: return new InvoiceStatusApproveHandler(_invoiceRepository);
                case InvoiceStatus.Cancelled: return new InvoiceStatusAnnulmentHandler(_invoiceRepository);
                default: return null;
            }
        }
    }
}
