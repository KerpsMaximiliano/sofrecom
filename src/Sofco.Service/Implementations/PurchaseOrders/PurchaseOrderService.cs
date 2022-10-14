using System;
using System.Collections.Generic;
using System.Text;
using Sofco.Core.Services.PurchaseOrders;
using Sofco.Domain.Utils;

namespace Sofco.Service.Implementations.PurchaseOrders
{
    public class PurchaseOrderService : IPurchaseOrderService
    {

        public Response<int> CreatePurchaseOrder(Domain.Models.PurchaseOrders.PurchaseOrder purchaseOrder)
        {
            var response = new Response<int>();
            response.Data = purchaseOrder.PurchaseOrderNumber;
            return response;
        }
    }
}
