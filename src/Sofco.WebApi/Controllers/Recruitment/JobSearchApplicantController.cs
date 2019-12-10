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

        [HttpPost("{applicantId}/{jobSearchId}/interview")]
        public IActionResult Interview(int applicantId, int jobSearchId, [FromBody] InterviewAddModel model)
        {
            var response = jobSearchApplicantService.AddInterview(applicantId, jobSearchId, model);

            return this.CreateResponse(response);
        }

        [HttpGet]
        public IActionResult Get([FromUri] JobSearchApplicantParameters parameters)
        {
            if (parameters.JobSearchId.HasValue)
            {
                var response = jobSearchApplicantService.GetByJobSearch(parameters.JobSearchId.Value);

                return this.CreateResponse(response);
            }

            //if (parameters.JobSearchId.HasValue)
            //{
            //    var response = jobSearchApplicantService.GetByJobSearch(parameters);

            //    return this.CreateResponse(response);
            //}

            var responseError = new Response();
            responseError.AddError(Resources.Common.ParametersNull);

            return this.CreateResponse(responseError);
        }

        [HttpPost("{jobSearchApplicantId}/file")]
        public async Task<IActionResult> File(int jobSearchApplicantId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await jobSearchApplicantService.AttachFile(jobSearchApplicantId, response, file);
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }
    }
}
