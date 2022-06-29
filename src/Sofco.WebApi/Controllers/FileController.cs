using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers
{
    [Route("api/file")]
    [Authorize]
    public class FileController : Controller
    {
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;

        public FileController(IFileService fileService, IOptions<FileConfig> fileOptions)
        {
            this.fileService = fileService;
            this.fileConfig = fileOptions.Value;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = fileService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/{type}")]
        public IActionResult GetFile(int id, int type)
        {
            var response = fileService.ExportFile(id, GetPathUrl(type));

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        private string GetPathUrl(int type)
        {
            switch (type)
            {
                case 1: return fileConfig.PurchaseOrdersPath;
                case 2: return fileConfig.CertificatesPath;
                case 3: return fileConfig.InvoicesPdfPath;
                case 4: return fileConfig.RecruitmentPath;
                case 5: return fileConfig.RequestNotePath;
                default: return string.Empty;
            }
        }
    }
}
