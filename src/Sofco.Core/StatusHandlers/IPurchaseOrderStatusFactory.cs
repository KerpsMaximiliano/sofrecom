using Sofco.Model.Enums;

namespace Sofco.Core.StatusHandlers
{
    public interface IPurchaseOrderStatusFactory
    {
        IPurchaseOrderStatusHandler GetInstance(PurchaseOrderStatus status);
    }
}
