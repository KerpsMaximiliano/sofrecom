using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
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

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var response = customerService.GetCustomersOptions();

            return this.CreateResponse(response);
        }

        [HttpGet("all/options")]
        public IActionResult GetAllOptions()
        {
            var response = customerService.GetCustomersOptions();

            return this.CreateResponse(response);
        }

        [HttpGet("options/currentManager")]
        public IActionResult GetOptionsByCurrentManager()
        {
            var response = customerService.GetCustomersOptionsByCurrentManager();

            return this.CreateResponse(response);
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = customerService.GetCustomers();

            return this.CreateResponse(response);
        }

        [HttpGet("{customerId}")]
        public IActionResult GetById(string customerId)
        {
            var response = customerService.GetCustomerById(customerId);

            return this.CreateResponse(response);
        }
    }
}
