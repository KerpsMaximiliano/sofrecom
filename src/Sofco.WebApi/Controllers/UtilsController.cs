using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.Domain.Utils;

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
            return Ok(utilsService.GetSectors());
        }

        [HttpGet("areas")]
        public IActionResult GetAreas()
        {
            return Ok(utilsService.GetAreas());
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

        [HttpGet("closeMonths")]
        public IActionResult GetCloseMonths()
        {
            var months = utilsService.GetCloseMonths();

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

        [HttpGet("userDelegateTypes")]
        public IActionResult GetUserDelegateTypes()
        {
            return Ok(utilsService.GetUserDelegateType());
        }

        [HttpGet("monthsReturn")]
        public IActionResult GetMonthsReturn()
        {
            return Ok(utilsService.GetMonthsReturn());
        }

        [HttpGet("creditCards")]
        public IActionResult GetCreditCards()
        {
            return Ok(utilsService.GetCreditCards());
        }

        [HttpGet("Banks")]
        public IActionResult GetBanks()
        {
            return Ok(utilsService.GetBanks());
        }

        [HttpGet("EmployeeProfile")]
        public IActionResult GetEmployeeProfiles()
        {
            return Ok(utilsService.GetEmployeeProfiles());
        }
    }
}
