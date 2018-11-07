using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/advancement")]
    [Authorize]
    public class AdvancementController : Controller
    {
        private readonly IAdvancementService advancementService;

        public AdvancementController(IAdvancementService advancementService)
        {
            this.advancementService = advancementService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdvancementModel model)
        {
            var response = advancementService.Add(model);

            return this.CreateResponse(response);
        }
    }
}
