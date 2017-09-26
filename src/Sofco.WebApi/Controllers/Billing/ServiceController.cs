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
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Config;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/service")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly CrmConfig _crmConfig;
        private readonly IUserService _userService;

        public ServiceController(IOptions<CrmConfig> crmOptions, IUserService userService)
        {
            _crmConfig = crmOptions.Value;
            _userService = userService;
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
            var username = User.Identity.Name.Split('@');
            var mail = $"{username[0]}@sofrecom.com.ar";

            var hasDirectorGroup = this._userService.HasDirectorGroup(mail);

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);

                HttpResponseMessage response;

                if (hasDirectorGroup)
                {
                    response = await client.GetAsync($"/api/service?idAccount={customerId}");
                }
                else
                {
                    response = await client.GetAsync($"/api/service?idAccount={customerId}&idManager={mail}");
                }

                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var services = JsonConvert.DeserializeObject<IList<ServiceCrm>>(stringResult);

                return services;
            }
        }
    }
}
