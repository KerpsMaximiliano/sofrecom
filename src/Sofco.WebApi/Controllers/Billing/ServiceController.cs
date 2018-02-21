﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Core.Services.Billing;
using Sofco.Domain.Crm.Billing;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/services")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly IServicesService servicesService;
        private readonly IPurchaseOrderService purchaseOrderService;

        public ServiceController(IServicesService servicesService, IPurchaseOrderService purchaseOrderService)
        {
            this.servicesService = servicesService;
            this.purchaseOrderService = purchaseOrderService;
        }

        [HttpGet("{customerId}/options")]
        public IActionResult GetOptions(string customerId)
        {
            try
            {
                var customers = this.servicesService.GetServices(customerId, this.GetUserMail(), this.GetUserName());
                var model = customers.Select(x => new SelectListItem { Value = x.Id, Text = x.Nombre }).OrderBy(x => x.Text);

                return Ok(model);
            }
            catch
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
            catch
            {
                return BadRequest(new List<CrmService>());
            }
        }

        [HttpGet("{serviceId}/account/{customerId}")]
        public IActionResult GetById(string serviceId, string customerId)
        {
            try
            {
                var services = this.servicesService.GetServices(customerId, this.GetUserMail(), this.GetUserName());

                var service = services.FirstOrDefault(x => x.Id.Equals(serviceId));

                if (service == null) return BadRequest();

                return Ok(service);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("{serviceId}/purchaseOrders")]
        public IActionResult GetPurchaseOrders(string serviceId)
        {
            var purchaseOrders = this.purchaseOrderService.GetByService(serviceId);

            return Ok(purchaseOrders.Select(x => new PurchaseOrderListItem(x)));
        }
    }
}
