using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.Model.Models.Billing;
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
        public IActionResult Post([FromBody]SolfacDelegate solfacDelegate)
        {
            var response = solfacDelegateService.Save(solfacDelegate);

            return this.CreateResponse(response);
        }
    }
}
