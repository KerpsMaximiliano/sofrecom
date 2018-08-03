using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Models.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/purchaseOrders/actives/delegates")]
    public class PurchaseOrderActiveDelegatesController : Controller
    {
        private readonly IPurchaseOrderActiveDelegateService service;

        public PurchaseOrderActiveDelegatesController(IPurchaseOrderActiveDelegateService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = service.GetAll();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserDelegate userDelegate)
        {
            var response = service.Save(userDelegate);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = service.Delete(id);

            return this.CreateResponse(response);
        }
    }
}
