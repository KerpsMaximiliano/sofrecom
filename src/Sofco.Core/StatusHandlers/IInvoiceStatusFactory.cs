using Sofco.Domain.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface IInvoiceStatusFactory
    {
        IInvoiceStatusHandler GetInstance(InvoiceStatus status);
    }
}
