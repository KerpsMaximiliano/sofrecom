using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Common;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

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

            return Ok(new CertificateEditModel(response.Data));
        }

        [HttpGet("client/{client}")]
        public IActionResult GetByClients(string client)
        {
            var certificates = certificateService.GetByClient(client);

            return Ok(certificates.Select(x => new Option { Id = x.Id, Text = x.Name }));
        }

        [HttpPost]
        public IActionResult Post([FromBody] CertificateModel model)
        {
            var domain = model.CreateDomain(sessionManager.GetUserName());

            var response = certificateService.Add(domain);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] CertificateEditModel model)
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

        [HttpGet("{id}/file")]
        public IActionResult GetFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.CertificatesPath);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
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
