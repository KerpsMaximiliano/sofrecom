using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Services.RequestNote;
using Sofco.WebApi.Extensions;
using System.Web.Http;

namespace Sofco.WebApi.Controllers.PurchaseOrders
{
    [Route("api/buyOrderRequestNote")]
    public class BuyOrderController : Controller
    {
        private readonly IBuyOrderService service;

        public BuyOrderController(IBuyOrderService service)
        {
            this.service = service;
        }


        [HttpGet("GetAll")]
        public IActionResult GetAll([FromUri] BuyOrderGridFilters filters)
        {
            return Ok(this.service.GetAll(filters));
        }
    }
}
