using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/settings")]
    [Authorize]
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
            var result = settingService.GetAll();

            return result.CreateResponse(this);
        }

        [HttpPost]
        public IActionResult Post([FromBody] List<GlobalSetting> globalSettings)
        {
            var result = settingService.Save(globalSettings);

            return result.CreateResponse(this);
        }
    }
}
