using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Common;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.ManagementReport
{
    [Route("api/currencyExchange")]
    [Authorize]
    public class CurrencyExchangeController : Controller
    {
        private readonly ICurrencyExchangeService currencyExchangeService;

        public CurrencyExchangeController(ICurrencyExchangeService currencyExchangeService)
        {
            this.currencyExchangeService = currencyExchangeService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] CurrencyExchangeAddModel model)
        {
            var response = currencyExchangeService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] CurrencyExchangeUpdateModel model)
        {
            var response = currencyExchangeService.Update(id, model);

            return this.CreateResponse(response);
        }

        [HttpGet("{startMonth}/{startYear}/{endMonth}/{endYear}")]
        public IActionResult Get(int startMonth, int startYear, int endMonth, int endYear)
        {
            var response = currencyExchangeService.Get(startMonth, startYear, endMonth, endYear);

            return this.CreateResponse(response);
        }
    }
}
