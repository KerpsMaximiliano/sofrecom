using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/service")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly CrmConfig _crmConfig;

        public ServiceController(IOptions<CrmConfig> crmOptions)
        {
            _crmConfig = crmOptions.Value;
        }

        [HttpGet("{customerId}/options")]
        public async Task<IActionResult> GetOptions(string customerId)
        {
            try
            {
                IList<ServiceCrm> customers = await GetServices(customerId);
                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre });

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> Get(string customerId)
        {
            try
            {
                IList<ServiceCrm> services = await GetServices(customerId);

                return Ok(services);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        private async Task<IList<ServiceCrm>> GetServices(string customerId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);
                var response = await client.GetAsync($"/api/service?idAccount={customerId}");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var services = JsonConvert.DeserializeObject<IList<ServiceCrm>>(stringResult);

                return services;
            }
        }
    }
}
