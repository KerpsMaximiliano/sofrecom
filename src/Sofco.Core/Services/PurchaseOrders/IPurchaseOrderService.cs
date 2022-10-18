using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Core.Services.PurchaseOrders
{
    public interface IPurchaseOrderService
    {
        Response<int> CreatePurchaseOrder(Domain.Models.PurchaseOrders.PurchaseOrder purchaseOrder);
    }
}
