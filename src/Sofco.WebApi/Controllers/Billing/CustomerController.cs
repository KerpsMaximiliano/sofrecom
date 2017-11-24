using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/customers")]
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerService customerService;

        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        [HttpGet("{userMail}/options")]
        public IActionResult GetOptions(string userMail)
        {
            try
            {
                var customers = this.customerService.GetCustomers(userMail, User.Identity.Name);

                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre });

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(new List<CrmCustomer>());
            }
        }

        [HttpGet("user/{userMail}")]
        public IActionResult Get(string userMail)
        {
            try
            {
                var customers = this.customerService.GetCustomers(userMail, this.GetUserName());

                return Ok(customers);
            }
            catch (Exception e)
            {
                return BadRequest(new List<CrmCustomer>());
            }
        }
        
        [HttpGet("{customerId}")]
        public IActionResult GetById(string customerId)
        {
            try
            {
                var response = this.customerService.GetCustomerById(customerId);

                if (response.HasErrors()) return BadRequest(response);

                return Ok(response.Data);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }
    }
}
