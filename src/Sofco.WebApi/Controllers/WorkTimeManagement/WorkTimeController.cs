using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.WorkTimeManagement;

namespace Sofco.WebApi.Controllers.WorkTimeManagement
{
    [Route("api/worktime")]
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
            var list = this.workTimeService.Get(date);

            return Ok(list);
        }
    }
}
