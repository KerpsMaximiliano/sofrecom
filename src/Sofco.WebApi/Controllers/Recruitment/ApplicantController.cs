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

        [HttpPost("search")]
        public IActionResult Search([FromBody] ApplicantSearchParameters parameter)
        {
            var response = applicantService.Search(parameter);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ApplicantAddModel model)
        {
            var response = applicantService.Update(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = applicantService.Get(id);

            return this.CreateResponse(response);
        }
    }
}