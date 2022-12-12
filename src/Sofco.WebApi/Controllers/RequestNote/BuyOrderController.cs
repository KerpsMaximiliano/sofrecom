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

        [HttpGet("States")]
        public IActionResult Get()
        {
            var response = service.GetStates();

            return this.CreateResponse(response);
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll([FromUri] BuyOrderGridFilters filters)
        {
            return Ok(this.service.GetAll(filters));
        }

        [HttpPost]
        public IActionResult Post([FromBody] BuyOrderModel model)
        {
            var response = service.Add(model);

            return this.CreateResponse(response);
        }
    }
}
