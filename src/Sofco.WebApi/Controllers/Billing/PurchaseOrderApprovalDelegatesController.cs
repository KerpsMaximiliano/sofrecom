using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Billing.PurchaseOrder;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Billing;
using Sofco.Core.Services.Billing.PurchaseOrder;
using Sofco.Core.Services.Common;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Billing
{
    [Authorize]
    [Route("api/purchaseOrders/approvals/delegates")]
    public class PurchaseOrderApprovalDelegatesController : Controller
    {
        private readonly IPurchaseOrderApprovalDelegateService purchaseOrderApprovalDelegateService;

        private readonly IUtilsService utilsService;

        public PurchaseOrderApprovalDelegatesController(IPurchaseOrderApprovalDelegateService purchaseOrderApprovalDelegateService, IUtilsService utilsService, IUserService userService)
        {
            this.purchaseOrderApprovalDelegateService = purchaseOrderApprovalDelegateService;
            this.utilsService = utilsService;
        }

        [HttpGet("areas")]
        public IActionResult GetAreas()
        {
            return Ok(utilsService.GetAreasByCurrentUser());
        }

        [HttpGet("sectors")]
        public IActionResult GetSectors()
        {
            var response = utilsService.GetSectorsByCurrentUser();

            return this.CreateResponse(response);
        }

        [HttpGet("compliances")]
        public IActionResult GetComplianceUsers()
        {
            var response = purchaseOrderApprovalDelegateService.GetComplianceUsers();

            return this.CreateResponse(response);
        }

        [HttpGet("dafs")]
        public IActionResult GetDafUsers()
        {
            var response = purchaseOrderApprovalDelegateService.GetDafUsers();

            return this.CreateResponse(response);
        }
    }
}
