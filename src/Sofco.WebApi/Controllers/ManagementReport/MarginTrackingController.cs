using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.ManagementReport;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.ManagementReport
{
    [Route("api/marginTracking")]
    [Authorize]
    public class MarginTrackingController : Controller
    {
        private readonly IMarginTrackingService marginTrackingService;

        public MarginTrackingController(IMarginTrackingService marginTrackingService)
        {
            this.marginTrackingService = marginTrackingService;
        }

        [HttpGet("{managementReportId}")]
        public IActionResult GetDetail(int managementReportId)
        {
            var response = marginTrackingService.Get(managementReportId);

            return this.CreateResponse(response);
        }
    }
}
