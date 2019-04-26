using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Authorize]
    [Route("api/paymentPending")]
    public class PaymentPendingController : Controller
    {
        private readonly IPaymentPendingService service;

        public PaymentPendingController(IPaymentPendingService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAllPaymentPending()
        {
            var response = service.GetAllPaymentPending();

            return this.CreateResponse(response);
        }
    }
}
