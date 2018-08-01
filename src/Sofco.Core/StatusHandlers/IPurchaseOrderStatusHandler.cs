using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Models.Billing;
using Sofco.Domain.Utils;

namespace Sofco.Core.StatusHandlers
{
    public interface IPurchaseOrderStatusHandler
    {
        void Validate(Response response, PurchaseOrderStatusParams model, PurchaseOrder purchaseOrder);
        void Save(PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model);
        string GetSuccessMessage(PurchaseOrderStatusParams model);
        void SendMail(PurchaseOrder purchaseOrder, PurchaseOrderStatusParams model);
    }
}
