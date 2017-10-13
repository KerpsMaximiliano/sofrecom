using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Reports;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/reports/solfacs")]
    [Authorize]
    public class SolfacReportController : Controller
    {
        private readonly ISolfacReportService solfacReportService;

        public SolfacReportController(ISolfacReportService solfacReportService)
        {
            this.solfacReportService = solfacReportService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var result = solfacReportService.Get(DateTime.Now, DateTime.Now.AddDays(30));

            return result.CreateResponse(this);
        }
    }
}
