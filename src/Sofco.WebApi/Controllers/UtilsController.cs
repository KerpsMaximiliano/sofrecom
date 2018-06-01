using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Model.Utils;

namespace Sofco.WebApi.Controllers
{
    [Route("api/utils")]
    [Authorize]
    public class UtilsController : Controller
    {
        private readonly IUtilsService utilsService;

        public UtilsController(IUtilsService utilsService)
        {
            this.utilsService = utilsService;
        }

        [HttpGet("sectors")]
        public IActionResult GetSectors()
        {
            var sectors = utilsService.GetSectors();

            return Ok(sectors.Select(x => new Option { Id = x.Id, Text = x.Text }).OrderBy(x => x.Text));
        }

        [HttpGet("employeeTypeEndReasons")]
        public IActionResult GetEmployeeTypeEndReasons()
        {
            var sectors = utilsService.GetEmployeeTypeEndReasons();

            return Ok(sectors.Select(x => new Option { Id = x.Id, Text = x.Text }).OrderBy(x => x.Text));
        }

        [HttpGet("months")]
        public IActionResult GetMonths()
        {
            var months = utilsService.GetMonths();

            return Ok(months);
        }

        [HttpGet("years")]
        public IActionResult GetYears()
        {
            var years = utilsService.GetYears();

            return Ok(years);
        }

        [HttpGet("currencies")]
        public IActionResult GetCurrencies()
        {
            var currencies = utilsService.GetCurrencies();

            return Ok(currencies);
        }
    }
}
