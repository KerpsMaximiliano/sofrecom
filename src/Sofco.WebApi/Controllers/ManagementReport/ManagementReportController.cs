using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.ManagementReport
{
    [Route("api/managementReport")]
    [Authorize]
    public class ManagementReportController : Controller
    {
        private readonly IManagementReportService managementReportService;

        public ManagementReportController(IManagementReportService managementReportService)
        {
            this.managementReportService = managementReportService;
        }

        [HttpGet("{serviceId}")]
        public IActionResult GetDetail(string serviceId)
        {
            var response = managementReportService.GetDetail(serviceId);

            return this.CreateResponse(response);
        }

        [HttpGet("{serviceId}/billing")]
        public IActionResult GetBilling(string serviceId)
        {
            var response = managementReportService.GetBilling(serviceId);

            return this.CreateResponse(response);
        }

        [HttpGet("{serviceId}/costDetail")]
        public IActionResult GetDetailCost(string serviceId)
        {
            var response = managementReportService.GetCostDetail(serviceId);

            return this.CreateResponse(response);
        }

        [HttpPost("costDetail")]
        public IActionResult PostDetailCost([FromBody] CostDetailModel model)
        {
            var response = managementReportService.UpdateCostDetail(model);

            return this.CreateResponse(response);
        }

        [HttpPost("{serviceId}/costDetailMonth")]
        public IActionResult PutDetailCostMonth([FromBody] CostDetailMonthModel model)
        {
            var response = managementReportService.UpdateCostDetailMonth(model);

            return this.CreateResponse(response);
        }

        [HttpGet("{serviceId}/costDetailMonth/{month}/{year}")]
        public IActionResult GetCostDetailMonth(string serviceId, int month, int year)
        {
            var response = managementReportService.GetCostDetailMonth(serviceId, month, year);

            return this.CreateResponse(response);
        }

        [HttpDelete("{contractedId}/contrated")]
        public IActionResult DeleteContracted(int contractedId)
        {
            var response = managementReportService.DeleteContracted(contractedId);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/dates")]
        public IActionResult UpdateDates(int id, [FromBody] ManagementReportUpdateDates model)
        {
            var response = managementReportService.UpdateDates(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet("otherResources")]
        public IActionResult GetOtherResources()
        {
            var response = managementReportService.GetOtherResources();

            return Ok(response);
        }

        [HttpGet("{idType}/otherResources/{idCostDetail}")]
        public IActionResult GetOtherByMonth(int idType, int idCostDetail)
        {
            var response = managementReportService.GetOtherTypeAndCostDetail(idType, idCostDetail);

            return Ok(response);
        }

        [HttpDelete("{id}/otherResources")]
        public IActionResult DeleteOtherResources(int id)
        {
            var response = managementReportService.DeleteOtherResource(id);

            return this.CreateResponse(response);
        }

        [HttpPost("tracingReport")]
        public IActionResult Report([FromBody] TracingModel tracing)
        {
            var response = managementReportService.CreateTracingReport(tracing);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpPut("send")]
        public IActionResult Send([FromBody] ManagementReportSendModel model)
        {
            var response = managementReportService.Send(model);

            return this.CreateResponse(response);
        }

        [HttpPut("close")]
        public IActionResult Close([FromBody] ManagementReportCloseModel model)
        {
            var response = managementReportService.Close(model);

            return this.CreateResponse(response);
        }

        [HttpPost("comments")]
        public IActionResult AddComment([FromBody] ManagementReportAddCommentModel model)
        {
            var response = managementReportService.AddComment(model);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/comments")]
        public IActionResult GetComments(int id)
        {
            var response = managementReportService.GetComments(id);

            return this.CreateResponse(response);
        }
    }
}
