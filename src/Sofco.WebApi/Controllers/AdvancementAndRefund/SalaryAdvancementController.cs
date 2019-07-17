using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Domain.Utils;
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

        [HttpPost("import")]
        public IActionResult Import(int analyticId)
        {
            var response = new Response();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                advancementService.Import(file, response);
            }
            else
            {
                response.AddError(Resources.Common.FileMustBeIncluded);
            }

            return this.CreateResponse(response);
        }
    }
}
