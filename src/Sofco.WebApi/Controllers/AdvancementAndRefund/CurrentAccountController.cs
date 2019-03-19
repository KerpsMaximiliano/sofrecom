using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/currentAccount")]
    [Authorize]
    public class CurrentAccountController : Controller
    {
        private readonly ICurrentAccountService currentAccountService;

        public CurrentAccountController(ICurrentAccountService currentAccountService)
        {
            this.currentAccountService = currentAccountService;
        }

        [HttpGet]
        public IActionResult GetAllPaymentPending()
        {
            var response = currentAccountService.Get();

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult UpdateMassive([FromBody] UpdateMassiveModel model)
        {
            var response = currentAccountService.UpdateMassive(model);

            return this.CreateResponse(response);
        }
    }
}
