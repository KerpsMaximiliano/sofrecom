using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.WorkTimeManagement
{
    [Route("api/worktimes/worktimeControls")]
    [Authorize]
    public class WorkTimeControlsController : Controller
    {
        private readonly IWorkTimeControlService service;

        public WorkTimeControlsController(IWorkTimeControlService service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult Get([FromBody] WorkTimeControlParams model)
        {
            var response = service.Get(model);

            return this.CreateResponse(response);
        }

        [HttpGet("analytics/options/currentManager")]
        public IActionResult GetByCurrentManager()
        {
            var response = service.GetAnalyticOptionsByCurrentManager();

            return this.CreateResponse(response);
        }

        [HttpGet("export")]
        public IActionResult ExportControlHoursReport()
        {
            var response = service.ExportControlHoursReport();

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }
    }
}
