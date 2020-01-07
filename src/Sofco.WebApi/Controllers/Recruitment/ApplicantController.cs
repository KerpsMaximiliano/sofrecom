using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Models.Recruitment;
using Sofco.Core.Services.Common;
using Sofco.Core.Services.Recruitment;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Recruitment
{
    [Route("api/applicant")]
    public class ApplicantController : Controller
    {
        private readonly IApplicantService applicantService;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;

        public ApplicantController(IApplicantService applicantService, IFileService fileService, IOptions<FileConfig> fileOptions)
        {
            this.applicantService = applicantService;
            this.fileService = fileService;
            this.fileConfig = fileOptions.Value;
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

        [HttpPost("{id}/register")]
        public IActionResult Register(int id, [FromBody] RegisterModel model)
        {
            var response = applicantService.Register(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = applicantService.Get(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/history")]
        public IActionResult GetHistory(int id)
        {
            var response = applicantService.GetApplicantHistory(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/files")]
        public IActionResult GetFiles(int id)
        {
            var response = applicantService.GetFiles(id);

            return this.CreateResponse(response);
        }

        [HttpGet("file/{id}")]
        public IActionResult GetFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.RecruitmentPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpPut("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody] ApplicantChangeStatusModel parameter)
        {
            var response = applicantService.ChangeStatus(id, parameter);

            return this.CreateResponse(response);
        }

        [HttpPost("{applicantId}/file")]
        public async Task<IActionResult> File(int applicantId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await applicantService.AttachFile(applicantId, response, file);
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }
    }
}