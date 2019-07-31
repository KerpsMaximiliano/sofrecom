using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.ManagementReport
{
    [Route("api/managementReportStaff")]
    [Authorize]
    public class ManagementReportStaffController : Controller
    {
        private readonly IManagementReportStaffService managementReportStaffService;

        public ManagementReportStaffController(IManagementReportStaffService managementReportStaffService)
        {
            this.managementReportStaffService = managementReportStaffService;
        }

        [HttpGet("{id}")]
        public IActionResult GetDetail(int id)
        {
            var response = managementReportStaffService.GetDetail(id);

            return this.CreateResponse(response);
        }

        [HttpPost("{id}/budget")]
        public IActionResult AddBudget(int id, [FromBody] BudgetItem model)
        {
            var response = managementReportStaffService.AddBudget(id, model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/budget")]
        public IActionResult UpdateBudget(int id, [FromBody] BudgetItem model)
        {
            var response = managementReportStaffService.UpdateBudget(id, model);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}/budget/{budgetId}")]
        public IActionResult DeleteBudget(int id, int budgetId)
        {
            var response = managementReportStaffService.DeleteBudget(id, budgetId);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/costDetailMonth/{month}/{year}")]
        public IActionResult GetDetail(int id, int month, int year)
        {
            var response = managementReportStaffService.GetCostDetailMonth(id, month, year);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/costDetailStaff")]
        public IActionResult GetDetailCostStaff(int id)
        {
            var response = managementReportStaffService.GetCostDetailStaff(id);

            return this.CreateResponse(response);
        }

        [HttpPost("costDetailStaff")]
        public IActionResult PostDetailCostStaff([FromBody] CostDetailStaffModel model)
        {
            var response = managementReportStaffService.UpdateCostDetailStaff(model);

            return this.CreateResponse(response);
        }

        [HttpPut("close")]
        public IActionResult Close([FromBody] ManagementReportCloseModel model)
        {
            var response = managementReportStaffService.Close(model);

            return this.CreateResponse(response);
        }
    }
}