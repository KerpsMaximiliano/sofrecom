﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/analytics")]
    public class AnalyticController : Controller
    {
        private readonly IAnalyticService analyticService;
        private readonly IRefundService refundService;

        public AnalyticController(IAnalyticService analyticServ, IRefundService refundService)
        {
            analyticService = analyticServ;
            this.refundService = refundService ?? throw new ArgumentNullException(nameof(refundService));
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = analyticService.GetAll().Select(x => new AnalyticSearchViewModel(x));

            var res = analyticService.GetByCurrentUser();

            return Ok(model);
        }

        [HttpGet("byManagerId")]
        public IActionResult GetByLoggedManagerId()
        {
            var model = analyticService.GetAll().Select(x => new AnalyticSearchViewModel(x));

            var res = analyticService.GetByLoggedManagerId();

            return Ok(res);
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = new List<AnalyticOption>();

            options.AddRange(analyticService.GetAll().Select(x => new AnalyticOption { Id = x.Id, Text = $"{x.Title} - {x.Name}", Title = x.Title }));

            return Ok(options);
        }

        [HttpGet("options/active")]
        public IActionResult GetOptionsActive()
        {
            var options = new List<AnalyticOption>();

            options.AddRange(analyticService.GetAllActives().Select(x => new AnalyticOption { Id = x.Id, Text = $"{x.Title} - {x.Name}", Title = x.Title }));

            return Ok(options);
        }

        [HttpGet("clients/{clientId}")]
        public IActionResult GetByClient(string clientId)
        {
            var options = new List<AnalyticOptionForOcModel>();

            options.AddRange(analyticService.GetByClient(clientId, false));

            return Ok(options);
        }

        [HttpGet("clients/{clientId}/actives")]
        public IActionResult GetActivesByClient(string clientId)
        {
            var options = new List<AnalyticOptionForOcModel>();

            options.AddRange(analyticService.GetByClient(clientId, true));

            return Ok(options);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = analyticService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new AnalyticModel(response.Data));
        }

        [HttpGet("onlyPendingRefunds/{id}")]
        public IActionResult GetOnlyPendingRefunds(int id)
        {
            var response = analyticService.GetByIdWithOnlyPendingRefunds(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new AnalyticModel(response.Data));
        }

        [HttpGet("title/{title}")]
        public IActionResult GetByTitle(string title)
        {
            var response = analyticService.GetByTitle(title);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new AnalyticModel(response.Data));
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
        public IActionResult Post([FromBody] AnalyticModel model)
        {
            var response = analyticService.Add(model.CreateDomain());

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] AnalyticModel model)
        {
            var response = analyticService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPut("daf")]
        public IActionResult PutDaf([FromBody] AnalyticModel model)
        {
            var response = analyticService.UpdateDaf(model.CreateDomainDaf());

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
            var analityc = analyticService.GetByIdWhitRefund(id);
            if (analityc.HasErrors())
                return BadRequest(analityc);

            var pendingrefunds = this.refundService.GetPendingByAnaliticId(id);

            if (pendingrefunds.Data != null && pendingrefunds.Data.Count > 0)
                return BadRequest(new AnalyticModel(analityc.Data));

            var response = analyticService.Close(id, AnalyticStatus.Close);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/closeForExpenses")]
        public IActionResult CloseForExpenses(int id)
        {
            var response = analyticService.Close(id, AnalyticStatus.CloseToExpenses);

            return this.CreateResponse(response);
        }

        [HttpGet("options/currentUser")]
        public IActionResult GetByCurrentUser()
        {
            var response = analyticService.GetByCurrentUser();

            return this.CreateResponse(response);
        }

        [HttpGet("options/currentUser/requestNote")]
        public IActionResult GetByCurrentUserRequestNote()
        {
            var response = analyticService.GetByCurrentUserRequestNote();

            return this.CreateResponse(response);
        }

        [HttpGet("options/currentManager")]
        public IActionResult GetByCurrentManager()
        {
            var response = analyticService.GetByCurrentManager();

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult GetByParameters([FromBody] AnalyticSearchParameters query)
        {
            var response = analyticService.Get(query);

            return this.CreateResponse(response);
        }

        [HttpPost("report")]
        public IActionResult Report([FromBody] List<int> analytics)
        {
            var response = analyticService.CreateReport(analytics);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("{id}/opportunities")]
        public IActionResult GetOpportunities(int id)
        {
            var response = analyticService.GetOpportunities(id);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/reopen")]
        public IActionResult Reopen(int id)
        {
            var response = analyticService.Reopen(id, AnalyticStatus.Open);

            return this.CreateResponse(response);
        }
    }
}
