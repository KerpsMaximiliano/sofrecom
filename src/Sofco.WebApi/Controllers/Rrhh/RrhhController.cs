using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Core.Services.Rrhh;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Rrhh
{
    [Route("api/rrhh")]
    [Authorize]
    public class RrhhController : Controller
    {
        private readonly IRrhhService rrhhService;
        private readonly IEmployeeUpdateService employeeUpdateService;

        public RrhhController(IRrhhService rrhhService, IEmployeeUpdateService employeeUpdateService)
        {
            this.rrhhService = rrhhService;
            this.employeeUpdateService = employeeUpdateService;
        }

        [HttpGet("tiger/txt/{allUsers}")]
        public IActionResult GetTigerTxt(bool allUsers)
        {
            var response = rrhhService.GenerateTigerTxt(allUsers);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "text/plain", string.Empty);
        }

        [HttpPut("{year}/{month}/socialCharges")]
        public IActionResult SocialCharges(int year, int month)
        {
            var response = rrhhService.UpdateSocialCharges(year, month);

            return this.CreateResponse(response);
        }

        [HttpPut("prepaid")]
        public IActionResult Prepaid()
        {
            var response = employeeUpdateService.UpdateSalaryAndPrepaids();

            return this.CreateResponse(response);
        }
    }
}
