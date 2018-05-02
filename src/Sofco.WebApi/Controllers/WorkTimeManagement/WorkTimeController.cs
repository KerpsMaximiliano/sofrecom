﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.WorkTimeManagement
{
    [Route("api/worktimes")]
    [Authorize]
    public class WorkTimeController : Controller
    {
        private readonly IWorkTimeService workTimeService;

        public WorkTimeController(IWorkTimeService workTimeService)
        {
            this.workTimeService = workTimeService;
        }

        [HttpGet("{date}")]
        public IActionResult Get(DateTime date)
        {
            var response = workTimeService.Get(date);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WorkTimeAddModel model)
        {
            var response = workTimeService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPost("hoursApproved")]
        public IActionResult GetHoursApproved([FromBody] WorktimeHoursApprovedParams model)
        {
            var response = workTimeService.GetHoursApproved(model);

            return this.CreateResponse(response);
        }

        [HttpPost("hoursPending")]
        public IActionResult GetHoursPending([FromBody] WorktimeHoursPendingParams model)
        {
            var response = workTimeService.GetHoursPending(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/approve")]
        public IActionResult Approve(int id)
        {
            var response = workTimeService.Approve(id);

            return this.CreateResponse(response);
        }

        [HttpPut("approve")]
        public IActionResult Approve([FromBody]List<int> hourIds)
        {
            var response = workTimeService.ApproveAll(hourIds);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/reject")]
        public IActionResult Reject(int id, [FromBody] WorkTimeRejectParams parameters)
        {
            var response = workTimeService.Reject(id, parameters.Comments);

            return this.CreateResponse(response);
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalytics()
        {
            var analytics = workTimeService.GetAnalytics();

            return Ok(analytics);
        }
    }
}
