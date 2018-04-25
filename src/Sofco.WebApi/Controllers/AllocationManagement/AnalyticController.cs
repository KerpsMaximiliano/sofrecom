using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Billing;
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
            var options = new List<AnalyticOption>();

            options.AddRange(analyticService.GetAllActives().Select(x => new AnalyticOption { Id = x.Id, Text = $"{x.Title} - {x.Name}", Title = x.Title }));

            return Ok(options);
        }

        [HttpGet("clients/{clientId}")]
        public IActionResult GetByClient(string clientId)
        {
            var options = new List<AnalyticOptionForOcModel>();

            options.AddRange(analyticService.GetByClient(clientId));

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

        [HttpGet("{id}/resources/timeline/{dateSince}/{months}")]
        public IActionResult GetTimelineResources(int id, DateTime dateSince, int months)
        {
            var responseResources = analyticService.GetTimelineResources(id, dateSince, months);

            var model = responseResources.Data.Select(x => new ResourceForAnalyticsModel(x));

            var response = new Response<IEnumerable<ResourceForAnalyticsModel>> { Data = model };

            response.AddMessages(responseResources.Messages);

            return Ok(response);
        }

        [HttpGet("{id}/resources")]
        public IActionResult GetResources(int id)
        {
            return Ok(analyticService.GetResources(id));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AnalyticViewModel model)
        {
            var response = await analyticService.Add(model.CreateDomain());

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

        [HttpPut("{id}/close")]
        public IActionResult Close(int id)
        {
            var response = analyticService.Close(id);

            return this.CreateResponse(response);
        }

        [HttpGet("options/currentUser")]
        public IActionResult GetByCurrentUser()
        {
            var response = analyticService.GetByCurrentUser();

            return this.CreateResponse(response);
        }
    }
}
