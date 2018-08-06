using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.Domain.DTO;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/hitos")]
    [Authorize]
    public class HitoController : Controller
    {
        private readonly IHitoService hitoService;

        public HitoController(IHitoService hitoService)
        {
            this.hitoService = hitoService;
        }

        [HttpPut]
        [Route("{id}/close")]
        public IActionResult Close(string id)
        {
            var response = hitoService.Close(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        [Route("split")]
        public async Task<IActionResult> Split([FromBody] HitoSplittedParams hito)
        {
            var response = await hitoService.SplitHito(hito);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] HitoSplittedParams hito)
        {
            var response = await hitoService.Create(hito);

            return this.CreateResponse(response);
        }
    }
}
