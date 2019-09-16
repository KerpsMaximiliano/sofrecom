using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.ManagementReport;
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

        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(string id)
        {
            var response = hitoService.Get(id);

            return this.CreateResponse(response);
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
        public IActionResult Split([FromBody] HitoParameters hito)
        {
            var response = hitoService.SplitHito(hito);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] HitoParameters hito)
        {
            var response = hitoService.Create(hito);

            return this.CreateResponse(response);
        }

        [HttpPatch]
        public IActionResult Patch([FromBody] UpdateResourceBillingRequest data)
        {
            var response = hitoService.Patch(data);

            return this.CreateResponse(response);
        }

        [HttpDelete("{hitoId}/{projectId}")]
        public IActionResult Delete(string hitoId, string projectId)
        {
            var response = hitoService.Delete(hitoId, projectId);

            return this.CreateResponse(response);
        }
    }
}
