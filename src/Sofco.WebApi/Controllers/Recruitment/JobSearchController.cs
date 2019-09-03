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
    }
}
