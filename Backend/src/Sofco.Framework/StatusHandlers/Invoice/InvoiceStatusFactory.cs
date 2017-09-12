using Sofco.Core.StatusHandlers;
using Sofco.Model.Enums;

namespace Sofco.Framework.StatusHandlers.Invoice
{
    public static class InvoiceStatusFactory
    {
        public static IInvoiceStatusHandler GetInstance(InvoiceStatus status)
        {
            switch (status)
            {
                case InvoiceStatus.Sent: return new InvoiceStatusSentHandler();
                case InvoiceStatus.Rejected: return new InvoiceStatusRejectHandler();
                case InvoiceStatus.Approved: return new InvoiceStatusApproveHandler();
                default: return null;
            }
        }
    }
}
