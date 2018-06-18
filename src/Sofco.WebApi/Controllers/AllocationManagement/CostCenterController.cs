using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/costCenter")]
    [Authorize]
    public class CostCenterController : Controller
    {
        private readonly ICostCenterService costCenterService;

        public CostCenterController(ICostCenterService costCenterServ)
        {
            costCenterService = costCenterServ;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = costCenterService.GetAll();

            return Ok(list.Select(x => new CostCenterModel(x)));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = costCenterService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new CostCenterModel(response.Data));
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var list = costCenterService.GetAll();

            return Ok(list.Select(x => new Option { Id = x.Id, Text = $"{x.Code}-{x.Letter} {x.Description}" }));
        }

        [HttpPost]
        public IActionResult Post([FromBody] AddCostCenterModel model)
        {
            var domain = model.CreateDomain();

            var response = costCenterService.Add(domain);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] EditCostCenterModel model)
        {
            var response = costCenterService.Edit(model.Id, model.Description);

            return this.CreateResponse(response);
        }

        [HttpPut]
        [Route("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = costCenterService.Active(id, active);

            return this.CreateResponse(response);
        }
    }
}
