using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/settings")]
    public class SettingsController : Controller
    {
        private readonly ISettingService settingService;

        public SettingsController(ISettingService settingService)
        {
            this.settingService = settingService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var respone = settingService.GetAll();

            return this.CreateResponse(respone);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Post([FromBody] List<Setting> settings)
        {
            var response = settingService.Save(settings);

            return this.CreateResponse(response);
        }

        [HttpGet("licenseTypes")]
        [Authorize]
        public IActionResult GetLicensesTypes()
        {
            var response = settingService.GetLicenseTypes();

            return Ok(response);
        }

        [HttpPost("{id}")]
        [Authorize]
        public IActionResult Post([FromBody] Setting settings)
        {
            var response = settingService.Save(settings);

            return this.CreateResponse(response);
        }
    }
}
