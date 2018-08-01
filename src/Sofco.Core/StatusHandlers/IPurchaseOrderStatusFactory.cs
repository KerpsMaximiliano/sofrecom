using Sofco.Domain.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface IPurchaseOrderStatusFactory
    {
        IPurchaseOrderStatusHandler GetInstance(PurchaseOrderStatus status);
    }
}
