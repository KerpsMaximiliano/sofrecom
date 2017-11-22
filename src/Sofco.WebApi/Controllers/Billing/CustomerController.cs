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
using Sofco.Core.Services;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/customers")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IUserService userService;
        private readonly CrmConfig crmConfig;
        private readonly ILoginService loginService;

        public CustomerController(IUserService userService, IOptions<CrmConfig> crmOptions, ILoginService loginService)
        {
            this.userService = userService;
            crmConfig = crmOptions.Value;
            this.loginService = loginService;
        }

        [HttpGet("{userMail}/options")]
        public async Task<IActionResult> GetOptions(string userMail)
        {
            try
            {
                var customers = await GetCustomers(userMail);

                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre });

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("user/{userMail}")]
        public async Task<IActionResult> Get(string userMail)
        {
            try
            {
                var customers = await GetCustomers(userMail);

                return Ok(customers);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetById(string customerId)
        {
            try
            {
                var customer = await GetCustomerById(customerId);

                if (customer.Id.Equals("00000000-0000-0000-0000-000000000000"))
                {
                    var response = new Model.Utils.Response();
                    response.Messages.Add(new Model.Utils.Message(Resources.Billing.Customer.NotFound, Model.Enums.MessageType.Error));

                    return BadRequest(response);
                }

                return Ok(customer);
            }
            catch
            {
                return BadRequest();
            }
        }

        private async Task<CustomerCrm> GetCustomerById(string customerId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);
                var response = await client.GetAsync($"/api/account/{customerId}");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<CustomerCrm>(stringResult);

                return customer;
            }
        }

        private async Task<IList<CustomerCrm>> GetCustomers(string userMail)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                var hasDirectorGroup = this.userService.HasDirectorGroup(userMail);

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
