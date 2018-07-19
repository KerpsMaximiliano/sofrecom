using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Reports;
using Sofco.Model.DTO;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Reports
{
    [Route("api/reports/purchaseOrders")]
    [Authorize]
    public class PurchaseOrderReportController : Controller
    {
        private readonly IPurchaseOrderReportService purchaseOrderReportService;

        public PurchaseOrderReportController(IPurchaseOrderReportService purchaseOrderReportService)
        {
            this.purchaseOrderReportService = purchaseOrderReportService;
        }

        [HttpPost]
        public IActionResult Get([FromBody] SearchPurchaseOrderParams parameters)
        {
            var response = purchaseOrderReportService.Get(parameters);

            return this.CreateResponse(response);
        }

        [HttpGet("analytics/options")]
        public IActionResult GetByCurrentManager()
        {
            var response = purchaseOrderReportService.GetAnalyticsByCurrentUser();

            return this.CreateResponse(response);
        }

        [HttpPost("actives")]
        public IActionResult GetActive([FromBody] SearchPurchaseOrderParams parameters)
        {
            var response = purchaseOrderReportService.GetActives(parameters);

            return this.CreateResponse(response);
        }
    }
}
