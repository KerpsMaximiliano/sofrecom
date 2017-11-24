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
    [Route("api/services")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly IServicesService servicesService;

        public ServiceController(IServicesService servicesService)
        {
            this.servicesService = servicesService;
        }

        [HttpGet("{customerId}/options")]
        public IActionResult GetOptions(string customerId)
        {
            try
            {
                var customers = this.servicesService.GetServices(customerId, this.GetUserMail(), this.GetUserName());
                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre });

                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{customerId}")]
        public IActionResult Get(string customerId)
        {
            try
            {
                var services = this.servicesService.GetServices(customerId, this.GetUserMail(), this.GetUserName());

                return Ok(services);
            }
            catch (Exception e)
            {
                return BadRequest(new List<CrmService>());
            }
        }
    }
}
