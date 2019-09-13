using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.ManagementReport;
using Sofco.Core.Services.ManagementReport;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.ManagementReport
{
    [Route("api/managementReportBillings")]
    [Authorize]
    public class ManagementReportBillingController : Controller
    {
        private readonly IManagementReportBillingService managementReportBillingService;

        public ManagementReportBillingController(IManagementReportBillingService managementReportBillingService)
        {
            this.managementReportBillingService = managementReportBillingService;
        }

        [HttpPut]
        public IActionResult PutDetailCostMonth([FromBody] UpdateValueModel model)
        {
            var response = managementReportBillingService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPut("data")]
        public IActionResult PutData([FromBody] UpdateBillingDataModel model)
        {
            var response = managementReportBillingService.UpdateData(model);

            return this.CreateResponse(response);
        }

        //[HttpPost("{idBilling}/resources")]
        //public IActionResult AddResources(int idBilling, [FromBody] IList<ResourceBillingRequestItem> resources)
        //{
        //    var response = managementReportBillingService.AddResources(idBilling, resources);

        //    return this.CreateResponse(response);
        //}

        [HttpGet("{idBilling}/{hitoId}/resources")]
        public IActionResult GetResources(int idBilling, string hitoId)
        {
            var response = managementReportBillingService.GetResources(idBilling, hitoId);

            return this.CreateResponse(response);
        }
    }
}
