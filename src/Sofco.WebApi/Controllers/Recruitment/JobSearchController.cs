using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/jobSearch")]
    public class JobSearchController : Controller
    {
        private readonly IJobSearchService jobSearchService;

        public JobSearchController(IJobSearchService jobSearchService)
        {
            this.jobSearchService = jobSearchService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] JobSearchAddModel model)
        {
            var response = jobSearchService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] JobSearchAddModel model)
        {
            var response = jobSearchService.Update(id, model);

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] JobSearchParameter parameter)
        {
            var response = jobSearchService.Search(parameter);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = jobSearchService.Get(id);

            return this.CreateResponse(response);
        }

        [HttpGet("applicants")]
        public IActionResult GetApplicants()
        {
            var response = jobSearchService.GetApplicants();

            return this.CreateResponse(response);
        }

        [HttpGet("{jobSearchId}/applicants")]
        public IActionResult GetApplicants(int jobSearchId)
        {
            var response = jobSearchService.GetByApplicantsRelated(jobSearchId);

            return this.CreateResponse(response);
        }

        [HttpGet("recruiters")]
        public IActionResult GetRecruiters()
        {
            var response = jobSearchService.GetRecruiters();

            return this.CreateResponse(response);
        }
         
        [HttpPut("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] JobSearchChangeStatusModel parameter)
        {
            var response = jobSearchService.ChangeStatus(id, parameter);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/history")]
        public IActionResult GetHistory(int id)
        {
            var response = jobSearchService.GetHistory(id);

            return this.CreateResponse(response);
        }
    }
}
