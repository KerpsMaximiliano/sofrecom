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
        public IActionResult Post([FromBody] List<GlobalSetting> globalSettings)
        {
            var response = settingService.Save(globalSettings);

            return this.CreateResponse(response);
        }

        [HttpGet("licenseTypes")]
        public IActionResult GetLicensesTypes()
        {
            var response = settingService.GetLicenseTypes();

            return Ok(response);
        }
    }
}
