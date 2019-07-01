using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/salaryAdvancement")]
    [Authorize]
    public class SalaryAdvancementController : Controller
    {
        private readonly ISalaryAdvancementService advancementService;

        public SalaryAdvancementController(ISalaryAdvancementService advancementService)
        {
            this.advancementService = advancementService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = advancementService.Get();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SalaryDiscountAddModel model)
        {
            var response = advancementService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = advancementService.Delete(id);

            return this.CreateResponse(response);
        }
    }
}
