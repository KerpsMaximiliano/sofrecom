using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/purchaseOrders/delegates")]
    public class PurchaseOrderDelegatesController : Controller
    {
        private readonly IPurchaseOrderDelegateService purchaseOrderDelegateService;

        public PurchaseOrderDelegatesController(IPurchaseOrderDelegateService purchaseOrderDelegateService)
        {
            this.purchaseOrderDelegateService = purchaseOrderDelegateService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = purchaseOrderDelegateService.GetAll();

            return this.CreateResponse(response);
        }
    }
}
