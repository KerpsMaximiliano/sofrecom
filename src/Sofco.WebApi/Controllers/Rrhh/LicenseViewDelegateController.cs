using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Models.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Authorize]
    [Route("api/licenses/views/delegates")]
    public class LicenseViewDelegateController : Controller
    {
        private readonly ILicenseViewDelegateService service;

        public LicenseViewDelegateController(ILicenseViewDelegateService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = service.GetAll();

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody]UserDelegate userDelegate)
        {
            var response = service.Save(userDelegate);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = service.Delete(id);

            return this.CreateResponse(response);
        }
    }
}
