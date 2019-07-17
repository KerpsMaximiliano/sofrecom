using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Rrhh;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Route("api/rrhh")]
    [Authorize]
    public class RrhhController : Controller
    {
        private readonly IRrhhService rrhhService;

        public RrhhController(IRrhhService rrhhService)
        {
            this.rrhhService = rrhhService;
        }

        [HttpGet("tiger/txt")]
        public IActionResult GetTigerTxt()
        {
            var response = rrhhService.GenerateTigerTxt();

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "text/plain", string.Empty);
        }

        [HttpPut("{year}/{month}/socialCharges")]
        public IActionResult SocialCharges(int year, int month)
        {
            var response = rrhhService.UpdateSocialCharges(year, month);

            return this.CreateResponse(response);
        }
    }
}
