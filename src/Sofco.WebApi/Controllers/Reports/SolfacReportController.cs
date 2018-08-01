using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Reports;
using Sofco.Domain.Models.Reports;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Reports
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

        [HttpPost]
        public IActionResult Get([FromBody] SolfacReportParams parameters)
        {
            var result = solfacReportService.Get(parameters);

            return result.CreateResponse(this);
        }
    }
}
