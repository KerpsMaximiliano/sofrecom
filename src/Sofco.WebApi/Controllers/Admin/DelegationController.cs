using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Common;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/delegation")]
    [Authorize]
    public class DelegationController : Controller
    {
        private readonly IDelegationService userDelegateService;

        public DelegationController(IDelegationService userDelegateService)
        {
            this.userDelegateService = userDelegateService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] DelegationAddModel model)
        {
            var response = userDelegateService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = userDelegateService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = userDelegateService.GetByUserId();

            return this.CreateResponse(response);
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalytics()
        {
            var response = userDelegateService.GetAnalytics();

            return this.CreateResponse(response);
        }

        [HttpGet("resources")]
        public IActionResult GetResources()
        {
            var response = userDelegateService.GetResources();

            return this.CreateResponse(response);
        }
    }
}
