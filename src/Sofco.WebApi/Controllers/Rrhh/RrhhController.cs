using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Rrhh;

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
    }
}
