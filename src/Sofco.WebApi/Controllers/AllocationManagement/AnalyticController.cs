﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Models.AllocationManagement;
using Sofco.Model.Utils;

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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = analyticService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new AnalyticSearchViewModel(response.Data));
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
    }
}
