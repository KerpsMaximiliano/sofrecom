using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Common;
using Sofco.Core.Services.Rrhh;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Route("api/licenses")]
    [Authorize]
    public class LicenseController : Controller
    {
        private readonly ILicenseTypeService licenseTypeService;
        private readonly ILicenseService licenseService;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;
        private readonly IUserService userService;

        public LicenseController(ILicenseTypeService licenseTypeService, ILicenseService licenseService, IFileService fileService, IUserService userService, IOptions<FileConfig> fileOptions)
        {
            this.licenseTypeService = licenseTypeService;
            this.licenseService = licenseService;
            this.fileService = fileService;
            fileConfig = fileOptions.Value;
            this.userService = userService;
        }

        [HttpGet("types")]
        public IActionResult GetOptions()
        {
            var options = licenseTypeService.GetOptions();

            var model = new LicenseTypeOptions
            {
                OptionsWithPayment = options.Where(x => x.WithPayment)
                    .Select(x => new Option { Id = x.Id, Text = x.Description }).ToList(),
                OptionsWithoutPayment = options.Where(x => !x.WithPayment)
                    .Select(x => new Option { Id = x.Id, Text = x.Description }).ToList()
            };

            return Ok(model);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = licenseService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] LicenseAddModel model)
        {
            var response = licenseService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] LicenseSearchParams parameters)
        {
            return Ok(licenseService.Search(parameters));
        }

        [HttpPut]
        [Route("type/{typeId}/days/{value}")]
        public IActionResult UpdateTypeValue(int typeId, int value)
        {
            var response = licenseTypeService.UpdateLicenseTypeDays(typeId, value);

            return this.CreateResponse(response);
        }

        [HttpGet]
        [Route("status/{statusId}")]
        public IActionResult GetByStatus(LicenseStatus statusId)
        {
            return Ok(licenseService.GetByStatus(statusId));
        }

        [HttpGet]
        [Route("manager/{managerId}")]
        public IActionResult GetByManager(int managerId)
        {
            return Ok(licenseService.GetByManager(managerId));
        }

        [HttpGet]
        [Route("employee/{employeeId}")]
        public IActionResult GetByEmployee(int employeeId)
        {
            return Ok(licenseService.GetByEmployee(employeeId));
        }

        [HttpGet]
        [Route("status/{statusId}/manager/{managerId}")]
        public IActionResult GetByManagerAndStatus(LicenseStatus statusId, int managerId)
        {
            return Ok(licenseService.GetByManagerAndStatus(statusId, managerId));
        }

        [HttpPost("{id}/file")]
        public async Task<IActionResult> File(int id)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                response = await licenseService.AttachFile(id, response, file);
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpDelete("file/{id}")]
        public IActionResult DeleteFile(int id)
        {
            var response = licenseService.DeleteFile(id);

            return this.CreateResponse(response);
        }

        [HttpGet("file/{id}")]
        public IActionResult GetFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.LicensesPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody]LicenseStatusChangeModel model)
        {
            var response = licenseService.ChangeStatus(id, model, null);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = licenseService.GetHistories(id);

            return Ok(histories);
        }

        [HttpPost]
        [Route("report")]
        public IActionResult Report([FromBody] ReportParams parameters)
        {
            try
            {
                var response = licenseService.GetLicenseReport(parameters);

                if (response.HasErrors())
                    return BadRequest(response);

                return File(response.Data, "application/octet-stream", string.Empty);
            }
            catch
            {
                var response = new Response();
                response.Messages.Add(new Message("Ocurrio un error al generar el excel", MessageType.Error));
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Route("{id}/fileDelivered")]
        public IActionResult FileDelivered(int id)
        {
            var response = licenseService.FileDelivered(id);

            return this.CreateResponse(response);
        }

        [HttpGet]
        [Route("authorizers")]
        public IActionResult GetAuthorizers()
        {
            var users = userService.GetAuthorizers();

            return Ok(users.Select(x => new Option() { Id = x.Id, Text = x.Name }));
        }
    } 
}
