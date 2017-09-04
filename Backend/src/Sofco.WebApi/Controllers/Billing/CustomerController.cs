using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sofco.Core.Services.Admin;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/customer")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly IUserService _userService;

        public CustomerController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{userMail}")]
        public async Task <IActionResult> Get(string userMail)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("http://sofrelab-iis1.cloudapp.net:4090");

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

                    return Ok(customers);
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
        }
    }
}
