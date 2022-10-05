using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.PurchaseOrders;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.PurchaseOrders
{
    [Route("api/purchaseOrderNotaPedido")]
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService service;

        public PurchaseOrderController(IPurchaseOrderService service)
        {
            this.service = service;
        }


        [HttpPost]
        public IActionResult CreatePurchaseOrder([FromBody] Domain.Models.PurchaseOrders.PurchaseOrder purchaseOrder)
        {
            var response = service.CreatePurchaseOrder(purchaseOrder);

            return this.CreateResponse(response);
        }

    }
}
