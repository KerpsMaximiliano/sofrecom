using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Models;
using Sofco.WebApi.Models.Billing;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Sofco.Core.Services.Billing;
using Sofco.Model.Models.Billing;
using Sofco.WebApi.Config;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/solfac")]
    public class SolfacController : Controller
    {
        private readonly IUtilsService _utilsService;
        private readonly ISolfacService _solfacService;

        public SolfacController(IUtilsService utilsService, ISolfacService solfacService)
        {
            _utilsService = utilsService;
            _solfacService = solfacService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var solfacs = _solfacService.GetAll();

            var list = solfacs.Select(x => new SolfacSearchDetail(x));

            return Ok(list);
        }

        [HttpGet]
        [Route("options")]
        public IActionResult FormOptions()
        {
            var options = new SolfacOptions
            {
                Currencies = _utilsService.GetCurrencies().Select(x => new Option<int>(x.Id, x.Text)).ToList(),
                DocumentTypes = _utilsService.GetDocumentTypes().Select(x => new Option<int>(x.Id, x.Text)).ToList(),
                ImputationNumbers = _utilsService.GetImputationNumbers().Select(x => new Option<int>(x.Id, x.Text)).ToList(),
                Provinces = new List<Option<int>> {new Option<int>(0, "Seleccione una opción")}
            };

            var provinces = _utilsService.GetProvinces().Where(x => x.Id != 1 && x.Id != 2).ToList();

            foreach (var province in provinces)
                options.Provinces.Add(new Option<int>(province.Id, province.Text));

            return Ok(options);
        }

        [HttpPost]
        public IActionResult Post([FromBody] SolfacViewModel model)
        {
            var errors = this.GetErrors();

            if (errors.HasErrors()) return BadRequest(errors);

            var domain = model.CreateDomain();

            var response = _solfacService.Add(domain);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
