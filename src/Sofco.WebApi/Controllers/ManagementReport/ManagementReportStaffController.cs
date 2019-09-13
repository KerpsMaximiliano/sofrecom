using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.Domain.Enums;
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
            var response = managementReportStaffService.SaveCostDetailStaff(model);

            return this.CreateResponse(response);
        }

        [HttpPost("costDetailMonth")]
        public IActionResult PostDetailCostMonthStaff([FromBody] CostDetailStaffMonthModel model)
        {
            var response = managementReportStaffService.SaveCostDetailStaffMonth(model);

            return this.CreateResponse(response);
        }

        [HttpPut("close")]
        public IActionResult Close([FromBody] ManagementReportCloseModel model)
        {
            var response = managementReportStaffService.Close(model);

            return this.CreateResponse(response);
        }

        [HttpGet("costDetailCategories")]
        public IActionResult GetCostDetailCategories()
        {
            var response = managementReportStaffService.GetCategories();

            return this.CreateResponse(response);
        }

        [HttpPost("generatePFA")]
        public IActionResult GeneratePFA1([FromBody] ManagementGeneratePFAModel model)
        {
            string enumPFA = EnumBudgetType.pfa1;
            if (model.TypePFA == 2)
            {
                enumPFA = EnumBudgetType.pfa2;
            }

            var response = managementReportStaffService.GeneratePFA(model.IdManagementReport, enumPFA);

            return this.CreateResponse(response);
        }
    }
}