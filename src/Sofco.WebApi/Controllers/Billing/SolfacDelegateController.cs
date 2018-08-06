using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Models.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/solfacs/delegates")]
    [Authorize]
    public class SolfacDelegateController : Controller
    {
        private readonly ISolfacDelegateService solfacDelegateService;

        public SolfacDelegateController(ISolfacDelegateService solfacDelegateService)
        {
            this.solfacDelegateService = solfacDelegateService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = solfacDelegateService.GetAll();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserDelegate userDelegate)
        {
            var response = solfacDelegateService.Save(userDelegate);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = solfacDelegateService.Delete(id);

            return this.CreateResponse(response);
        }
    }
}
