using System.Collections.Generic;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Domain.Utils;

namespace Sofco.Core.Services.Billing.PurchaseOrder
{
    public interface IPurchaseOrderStatusService
    {
        Response ChangeStatus(int id, PurchaseOrderStatusParams model);

        Response Close(int id, PurchaseOrderStatusParams model);

        Response MakeAdjustment(int id, PurchaseOrderAdjustmentModel model);
    }
}
