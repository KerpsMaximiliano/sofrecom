using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Route("api/prepaid")]
    [Authorize]
    public class PrepaidController : Controller
    {
        private readonly IPrepaidService prepaidService;

        public PrepaidController(IPrepaidService prepaidService)
        {
            this.prepaidService = prepaidService;
        }

        [HttpPost("{prepaidId}/import/{yearId}/{monthId}")]
        public IActionResult Import(int prepaidId, int yearId, int monthId)
        {
            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                var response = this.prepaidService.Import(prepaidId, yearId, monthId, file);

                return this.CreateResponse(response);
            }
            else
            {
                var response = new Response();
                response.AddError(Resources.Common.FileMustBeIncluded);
                return this.CreateResponse(response);
            }
        }

        [HttpGet("{yearId}/{monthId}")]
        public IActionResult Import(int yearId, int monthId)
        {
            var response = this.prepaidService.GetDashboard(yearId, monthId);

            return this.CreateResponse(response);
        }
    }
}
