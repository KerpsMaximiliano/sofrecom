using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.AllocationManagement;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/analytics")]
    public class AnalyticController : Controller
    {
        private readonly IAnalyticService analyticService;

        public AnalyticController(IAnalyticService analyticServ)
        {
            analyticService = analyticServ;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = analyticService.GetAll().Select(x => new AnalyticSearchViewModel(x));

            return Ok(model);
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = new List<Option> { new Option { Id = 0, Text = "Seleccione una opcion" } };

            options.AddRange(analyticService.GetAll().Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }));

            return Ok(options);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = analyticService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new AnalyticViewModel(response.Data));
        }

        [HttpGet("formOptions")]
        public IActionResult GetFormOptions()
        {
            return Ok(analyticService.GetOptions());
        }

        [HttpGet("{id}/resources")]
        public IActionResult GetResources(int id)
        {
            var responseResources = analyticService.GetResources(id);

            var model = responseResources.Data.Select(x => new ResourceForAnalyticsModel(x));

            var response = new Response<IEnumerable<ResourceForAnalyticsModel>> { Data = model };

            response.AddMessages(responseResources.Messages);

            return Ok(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AnalyticViewModel model)
        {
            var response = analyticService.Add(model.CreateDomain());

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] AnalyticEditViewModel model)
        {
            var response = analyticService.Update(model.CreateDomain());

            return this.CreateResponse(response);
        }

        [HttpGet("title/costcenter/{costCenterId}")]
        public IActionResult GetNewTitle(int costCenterId)
        {
            var response = analyticService.GetNewTitle(costCenterId);

            return this.CreateResponse(response);
        }
    }
}
