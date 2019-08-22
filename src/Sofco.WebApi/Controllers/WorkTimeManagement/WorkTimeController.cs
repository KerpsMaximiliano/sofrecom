using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.WorkTimeManagement;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.WorkTimeManagement
{
    [Route("api/worktimes")]
    [Authorize]
    public class WorkTimeController : Controller
    {
        private readonly IWorkTimeService workTimeService;
        private readonly IWorkTimeReportService workTimeReportService;

        public WorkTimeController(IWorkTimeService workTimeService, IWorkTimeReportService workTimeReportService)
        {
            this.workTimeService = workTimeService;
            this.workTimeReportService = workTimeReportService;
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
            var response = workTimeService.Save(model);

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
        public IActionResult Approve([FromBody]List<HoursToApproveModel> hourIds)
        {
            var response = workTimeService.ApproveAll(hourIds);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/reject")]
        public IActionResult Reject(int id, [FromBody] WorkTimeRejectParams parameters)
        {
            var response = workTimeService.Reject(id, parameters.Comments, false);

            return this.CreateResponse(response);
        }

        [HttpPut("reject")]
        public IActionResult RejectAll([FromBody] WorkTimeRejectParams parameters)
        {
            var response = workTimeService.RejectAll(parameters);

            return this.CreateResponse(response);
        }

        [HttpPut("send")]
        public IActionResult Send()
        {
            var response = workTimeService.Send();

            return this.CreateResponse(response);
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalytics()
        {
            var analytics = workTimeService.GetAnalytics();

            return Ok(analytics);
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            var status = workTimeService.GetStatus();

            return Ok(status);
        }

        [HttpPost("report")]
        public IActionResult Report([FromBody] ReportParams parameters)
        {
            var response = workTimeReportService.CreateReport(parameters);

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchParams parameters)
        {
            var response = workTimeService.Search(parameters);

            return this.CreateResponse(response); 
        }

        [HttpDelete("{id}")]
        public IActionResult Search(int id)
        {
            var response = workTimeService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpPost("import/{analyticId}")]
        public IActionResult Import(int analyticId)
        {
            var response = new Response<IList<WorkTimeImportResult>>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                workTimeService.Import(analyticId, file, response);
            }
            else
            {
                response.AddError(Resources.Common.FileMustBeIncluded);
            }

            return this.CreateResponse(response);
        }

        [HttpGet("export/template/{analyticId}/{periodId}")]
        public IActionResult ExportTemplate(int analyticId, int periodId)
        {
            var file = workTimeService.ExportTemplate(analyticId, periodId);

            return File(file, "application/octet-stream", string.Empty);
        }
    }
}
