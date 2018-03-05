using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Rrhh;
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
                    .Select(x => new Option {Id = x.Id, Text = x.Description}).ToList(),
                OptionsWithoutPayment = options.Where(x => !x.WithPayment)
                    .Select(x => new Option {Id = x.Id, Text = x.Description}).ToList()
            };

            return Ok(model);
        }

        [HttpPost]
        public IActionResult Post([FromBody] LicenseAddModel model)
        {
            var response = licenseService.Add(model.CreateDomain());

            return this.CreateResponse(response);
        }

        [HttpPut]
        [Route("type/{typeId}/days/{value}")]
        public IActionResult UpdateTypeValue(int typeId, int value)
        {
            var response = licenseTypeService.UpdateLicenseTypeDays(typeId, value);

            return this.CreateResponse(response);
        }

        [HttpPost("{id}/file")]
        public async Task<IActionResult> File(int id)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await licenseService.AttachFile(id, response, file);
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }
    }
}
