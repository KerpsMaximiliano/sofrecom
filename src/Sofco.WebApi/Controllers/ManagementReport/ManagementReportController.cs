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

        [HttpPost("{serviceId}/costDetail")]
        public IActionResult PutDetailCost([FromBody] CostDetailModel model)
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
    }
}
