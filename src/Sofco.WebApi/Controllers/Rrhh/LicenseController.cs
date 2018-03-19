using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Common;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Rrhh;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Route("api/licenses")]
    [Authorize]
    public class LicenseController : Controller
    {
        private readonly ILicenseTypeService licenseTypeService;
        private readonly ILicenseService licenseService;

        public LicenseController(ILicenseTypeService licenseTypeService, ILicenseService licenseService)
        {
            this.licenseTypeService = licenseTypeService;
            this.licenseService = licenseService;
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
            var response = licenseService.Add(model.CreateDomain());

            if (model.IsRrhh && model.EmployeeLoggedId != model.EmployeeId)
            {
                var statusParams = new LicenseStatusChangeModel
                {
                    Status = LicenseStatus.ApprovePending,
                    UserId = model.UserId,
                    IsRrhh = model.IsRrhh
                };

                var statusResponse = licenseService.ChangeStatus(Convert.ToInt32(response.Data), statusParams);
                response.AddMessages(statusResponse.Messages);
            }
            else
            {
                var statusParams = new LicenseStatusChangeModel
                {
                    Status = LicenseStatus.AuthPending,
                    UserId = model.UserId,
                    IsRrhh = model.IsRrhh
                };

                var statusResponse = licenseService.ChangeStatus(Convert.ToInt32(response.Data), statusParams);
                response.AddMessages(statusResponse.Messages);
            }

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

        [HttpPost]
        [Route("{id}/status")]
        public IActionResult ChangeStatus(int id, [FromBody]LicenseStatusChangeModel model)
        {
            var response = licenseService.ChangeStatus(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var histories = licenseService.GetHistories(id);

            return Ok(histories);
        }
    }
}
