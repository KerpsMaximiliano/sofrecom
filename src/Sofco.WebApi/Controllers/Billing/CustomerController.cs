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
    [Route("api/customer")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;
        private readonly CrmConfig _crmConfig;

        public CustomerController(IUserService userService, IOptions<CrmConfig> crmOptions)
        {
            _userService = userService;
            _crmConfig = crmOptions.Value;
        }

        [HttpGet("{userMail}/options")]
        public async Task<IActionResult> GetOptions(string userMail)
        {
            try
            {
                IList<CustomerCrm> customers = await GetCustomers(userMail);
                var model = customers.Select(x => new SelectListItem {Value = x.Id, Text = x.Nombre});

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{userMail}")]
        public async Task <IActionResult> Get(string userMail)
        {
            try
            {
                IList<CustomerCrm> customers = await GetCustomers(userMail);

                return Ok(customers);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        private async Task<IList<CustomerCrm>> GetCustomers(string userMail)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(_crmConfig.Url);

                var hasDirectorGroup = this._userService.HasDirectorGroup(userMail);

                HttpResponseMessage response;

                if (hasDirectorGroup)
                {
                    response = await client.GetAsync($"/api/account");
                }
                else
                {
                    response = await client.GetAsync($"/api/account?idManager={userMail}");
                }

                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<IList<CustomerCrm>>(stringResult);
                return customers;
            }
        }
    }
}
