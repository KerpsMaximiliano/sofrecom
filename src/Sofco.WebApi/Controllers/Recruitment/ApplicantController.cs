using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/applicant")]
    public class ApplicantController : Controller
    {
        private readonly IApplicantService applicantService;

        public ApplicantController(IApplicantService applicantService)
        {
            this.applicantService = applicantService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] ApplicantAddModel model)
        {
            var response = applicantService.Add(model);

            return this.CreateResponse(response);
        }
    }
}
