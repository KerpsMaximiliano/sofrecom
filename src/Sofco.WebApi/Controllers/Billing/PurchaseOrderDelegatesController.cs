using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/purchaseOrders/delegates")]
    public class PurchaseOrderDelegatesController : Controller
    {
        private readonly IPurchaseOrderDelegateService purchaseOrderDelegateService;

        private readonly IUtilsService utilsService;

        public PurchaseOrderDelegatesController(IPurchaseOrderDelegateService purchaseOrderDelegateService, IUtilsService utilsService)
        {
            this.purchaseOrderDelegateService = purchaseOrderDelegateService;
            this.utilsService = utilsService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = purchaseOrderDelegateService.GetAll();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody]PurchaseOrderDelegateModel userDelegate)
        {
            var response = purchaseOrderDelegateService.Save(userDelegate);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = purchaseOrderDelegateService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpGet("areas")]
        public IActionResult GetAreas()
        {
            return Ok(utilsService.GetAreasByCurrentUser());
        }

        [HttpGet("sectors")]
        public IActionResult GetSectors()
        {
            return Ok(utilsService.GetSectorsByCurrentUser());
        }
    }
}
