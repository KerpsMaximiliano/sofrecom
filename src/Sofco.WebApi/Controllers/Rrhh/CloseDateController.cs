using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Models.Rrhh;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Route("api/closeDate")]
    [Authorize]
    public class CloseDateController : Controller
    {
        private readonly ICloseDateService closeDateService;

        public CloseDateController(ICloseDateService closeDateService)
        {
            this.closeDateService = closeDateService;
        }

        [HttpGet("{startMonth}/{startYear}/{endMonth}/{endYear}")]
        public IActionResult Get(int startMonth, int startYear, int endMonth, int endYear)
        {
            var response = closeDateService.Get(startMonth, startYear, endMonth, endYear);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] IList<CloseDate> model)
        {
            var response = closeDateService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpGet("GetFirstBeforeNextMonth")]
        public IActionResult GetFirstBeforeNextMonth()
        {
            var response = closeDateService.GetFirstBeforeNextMonth();

            return Ok(response);
        }
    }
}
