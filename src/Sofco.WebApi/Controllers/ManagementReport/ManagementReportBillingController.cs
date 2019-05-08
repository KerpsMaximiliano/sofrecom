using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPut("{id}")]
        public IActionResult PutDetailCostMonth(int id, [FromBody] decimal value)
        {
            var response = managementReportBillingService.Update(id, value);

            return this.CreateResponse(response);
        }
    }
}
