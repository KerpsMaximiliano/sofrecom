using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/jobSearchApplicant")]
    public class JobSearchApplicantController : Controller
    {
        private readonly IJobSearchApplicantService jobSearchApplicantService;

        public JobSearchApplicantController(IJobSearchApplicantService jobSearchApplicantService)
        {
            this.jobSearchApplicantService = jobSearchApplicantService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] JobSearchApplicantAddModel model)
        {
            var response = jobSearchApplicantService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPost("{applicantId}/{jobSearchId}/{date}/{reasonId}/interview")]
        public IActionResult Interview(int applicantId, int jobSearchId, DateTime date, int reasonId, [FromBody] InterviewAddModel model)
        {
            var response = jobSearchApplicantService.AddInterview(applicantId, jobSearchId, date, reasonId, model);

            return this.CreateResponse(response);
        }

        [HttpGet]
        public IActionResult Get([FromUri] JobSearchApplicantParameters parameters)
        {
            if (parameters.JobSearchId.HasValue && parameters.JobSearchId.Value > 0)
            {
                var response = jobSearchApplicantService.GetByJobSearch(parameters.JobSearchId.Value);

                return this.CreateResponse(response);
            }

            if (parameters.ApplicantId.HasValue && parameters.ApplicantId.Value > 0)
            {
                var response = jobSearchApplicantService.GetByApplicant(parameters.ApplicantId.Value);

                return this.CreateResponse(response);
            }

            var responseError = new Response();
            responseError.AddError(Resources.Common.ParametersNull);

            return this.CreateResponse(responseError);
        }
    }
}
