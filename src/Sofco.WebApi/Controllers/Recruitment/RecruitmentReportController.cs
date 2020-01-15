using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/recruitment/report")]
    public class RecruitmentReportController : Controller
    {
        private readonly IRecruitmentReportService recruitmentReportService;

        public RecruitmentReportController(IRecruitmentReportService recruitmentReportService)
        {
            this.recruitmentReportService = recruitmentReportService;
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] RecruitmentReportParameters parameter)
        {
            var response = recruitmentReportService.Search(parameter);

            return this.CreateResponse(response);
        }
    }
}
