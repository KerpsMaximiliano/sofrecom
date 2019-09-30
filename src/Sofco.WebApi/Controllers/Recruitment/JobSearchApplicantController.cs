using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Recruitment;
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

        //[HttpPost]
        //public IActionResult Post([FromBody] ApplicantAddModel model)
        //{
        //    var response = jobSearchApplicantService.Add(model);

        //    return this.CreateResponse(response);
        //}

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
    }
}
