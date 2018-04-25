using System;
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
    }
}
