using Sofco.Model.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface IInvoiceStatusFactory
    {
        IInvoiceStatusHandler GetInstance(InvoiceStatus status);
    }
}
