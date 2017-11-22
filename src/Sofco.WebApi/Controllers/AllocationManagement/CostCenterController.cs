using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Models.AllocationManagement;

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

            return Ok(list.Select(x => new CostCenterViewModel(x)));
        }

        [HttpPost]
        public IActionResult Post([FromBody] AddCostCenterViewModel model)
        {
            var domain = model.CreateDomain();

            var response = costCenterService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
