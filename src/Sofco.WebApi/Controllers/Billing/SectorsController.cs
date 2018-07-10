using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Admin;
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = sectorService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SectorAdminModel model)
        {
            var response = this.sectorService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] SectorAdminModel model)
        {
            var response = this.sectorService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = this.sectorService.Active(id, active);

            return this.CreateResponse(response);
        }
    }
}
