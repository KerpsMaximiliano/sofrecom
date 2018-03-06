using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Model.DTO;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/certificates")]
    public class CertificateController : Controller
    {
        private readonly ICertificateService certificateService;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;
        private readonly ISessionManager sessionManager;

        public CertificateController(ICertificateService certificateService, IFileService fileService, IOptions<FileConfig> fileOptions, ISessionManager sessionManager)
        {
            this.certificateService = certificateService;
            this.fileService = fileService;
            this.sessionManager = sessionManager;
            this.fileConfig = fileOptions.Value;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = certificateService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new CertificateEditViewModel(response.Data));
        }

        [HttpGet("client/{client}")]
        public IActionResult GetByClients(string client)
        {
            var certificates = certificateService.GetByClient(client);

            return Ok(certificates.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));
        }

        [HttpPost]
        public IActionResult Post([FromBody] CertificateViewModel model)
        {
            var domain = model.CreateDomain(sessionManager.GetUserName());

            var response = certificateService.Add(domain);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] CertificateEditViewModel model)
        {
            var domain = model.CreateDomain(sessionManager.GetUserName());

            var response = certificateService.Update(domain);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}/file")]
        public IActionResult DeleteFile(int id)
        {
            var response = certificateService.DeleteFile(id);

            return this.CreateResponse(response);
        }

        [HttpPost("{certificateId}/file")]
        public async Task<IActionResult> File(int certificateId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await certificateService.AttachFile(certificateId, response, file, sessionManager.GetUserName());
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpGet("export/{id}")]
        public IActionResult ExportFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.CertificatesPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] SearchCertificateParams parameters)
        {
            var certificates = certificateService.Search(parameters);

            var response = new Response<List<CertificateListItem>>();
            response.Data = certificates.Select(x => new CertificateListItem(x)).ToList();

            if (!certificates.Any())
                response.AddWarning(Resources.Billing.PurchaseOrder.SearchEmpty);

            return Ok(response);
        }
    }
}
