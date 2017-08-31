using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/service")]
    [Authorize]
    public class ServiceController : Controller
    {
        [HttpGet("{customerId}")]
        public async Task<IActionResult> Get(string customerId)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://sofrelab-iis1.cloudapp.net:4090");
                    var response = await client.GetAsync($"/api/service?idAccount={customerId}");
                    response.EnsureSuccessStatusCode();

                    var stringResult = await response.Content.ReadAsStringAsync();
                    var services = JsonConvert.DeserializeObject<IList<ServiceCrm>>(stringResult);

                    return Ok(services);
                }
                catch (Exception e)
                {
                    return BadRequest();
                }
            }
        }
    }
}
