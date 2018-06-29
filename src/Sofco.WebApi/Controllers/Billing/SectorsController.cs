using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/sectors")]
    public class SectorsController : Controller
    {
        private readonly ISectorService sectorService;

        public SectorsController(ISectorService sectorService)
        {
            this.sectorService = sectorService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = sectorService.GetAll();

            return this.CreateResponse(response);
        }
    }
}
