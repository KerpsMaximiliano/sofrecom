using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.ManagementReport
{
    [Route("api/managementReportBillings")]
    [Authorize]
    public class ManagementReportBillingController : Controller
    {
        private readonly IManagementReportBillingService managementReportBillingService;

        public ManagementReportBillingController(IManagementReportBillingService managementReportBillingService)
        {
            this.managementReportBillingService = managementReportBillingService;
        }

        [HttpPut]
        public IActionResult PutDetailCostMonth([FromBody] UpdateValueModel model)
        {
            var response = managementReportBillingService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPut("data")]
        public IActionResult PutData([FromBody] UpdateBillingDataModel model)
        {
            var response = managementReportBillingService.UpdateData(model);

            return this.CreateResponse(response);
        }
    }
}
