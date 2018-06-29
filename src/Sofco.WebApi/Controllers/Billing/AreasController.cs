using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/areas")]
    public class AreasController : Controller
    {
        private readonly IAreaService areaService;

        public AreasController(IAreaService areaService)
        {
            this.areaService = areaService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = areaService.GetAll();

            return this.CreateResponse(response);
        }
    }
}
