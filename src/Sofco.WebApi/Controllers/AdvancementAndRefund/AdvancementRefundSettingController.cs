using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Common;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/advancementRefund/setting")]
    [Authorize]
    public class AdvancementRefundSettingController : Controller
    {
        private readonly IAdvancementRefundSettingService settingService;

        public AdvancementRefundSettingController(IAdvancementRefundSettingService settingService)
        {
            this.settingService = settingService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] SettingModel model)
        {
            var response = settingService.Save(model);

            return this.CreateResponse(response);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = settingService.Get();

            return this.CreateResponse(response);
        }
    }
}
