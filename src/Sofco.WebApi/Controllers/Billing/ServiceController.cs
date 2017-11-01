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
using Sofco.Core.Config;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/services")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly CrmConfig crmConfig;
        private readonly IUserService userService;

        public ServiceController(IOptions<CrmConfig> crmOptions, IUserService userService)
        {
            crmConfig = crmOptions.Value;
            this.userService = userService;
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
            var hasDirectorGroup = this.userService.HasDirectorGroup(this.GetUserMail());

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                HttpResponseMessage response;

                if (hasDirectorGroup)
                {
                    response = await client.GetAsync($"/api/service?idAccount={customerId}");
                }
                else
                {
                    response = await client.GetAsync($"/api/service?idAccount={customerId}&idManager={this.GetUserMail()}");
                }

                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var services = JsonConvert.DeserializeObject<IList<ServiceCrm>>(stringResult);

                return services;
            }
        }
    }
}
