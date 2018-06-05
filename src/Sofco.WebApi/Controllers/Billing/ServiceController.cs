using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Billing;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.AllocationManagement;
using Sofco.WebApi.Models.Billing;

namespace Sofco.WebApi.Controllers.Billing
{
    [Route("api/services")]
    [Authorize]
    public class ServiceController : Controller
    {
        private readonly IServicesService servicesService;
        private readonly IProjectService projectService;
        private readonly IPurchaseOrderService purchaseOrderService;

        public ServiceController(IServicesService servicesService, IProjectService projectService, IPurchaseOrderService purchaseOrderService)
        {
            this.servicesService = servicesService;
            this.projectService = projectService;
            this.purchaseOrderService = purchaseOrderService;
        }

        [HttpGet("{customerId}/options")]
        public IActionResult GetOptions(string customerId)
        {
            var response = servicesService.GetServicesOptions(customerId);

            return this.CreateResponse(response);
        }

        [HttpGet("{customerId}")]
        public IActionResult Get(string customerId)
        {
            var response = servicesService.GetServices(customerId);

            return this.CreateResponse(response);
        }

        [HttpGet("{serviceId}/account/{customerId}")]
        public IActionResult GetById(string serviceId, string customerId)
        {
            var response = servicesService.GetService(serviceId, customerId);

            return this.CreateResponse(response);
        }

        [HttpGet("{serviceId}/opportunities")]
        public IActionResult GetOpportunities(string serviceId)
        {
            var respone = projectService.GetOpportunities(serviceId);

            return this.CreateResponse(respone);
        }

        [HttpGet("{serviceId}/analytic")]
        public IActionResult GetAnalyticByService(string serviceId)
        {
            var analytic = servicesService.GetAnalyticByService(serviceId);

            var model = new AnalyticViewModel();

            if (analytic != null)
            {
                model.Id = analytic.Id;
                model.Name = analytic.Name;
            }

            return Ok(model);
        }

        [HttpGet("{serviceId}/purchaseOrders")]
        public IActionResult GetPurchaseOrders(string serviceId)
        {
            var purchaseOrders = purchaseOrderService.GetByService(serviceId);

            return Ok(purchaseOrders.Select(x => new PurchaseOrderListItem(x)));
        }
    }
}
