using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/workTimeApprovals/approvers")]
    [Authorize]
    public class WorkTimeApprovalsApproversController : Controller
    {
        private readonly IWorkTimeApprovalApproverService approverService;

        public WorkTimeApprovalsApproversController(IWorkTimeApprovalApproverService approverService)
        {
            this.approverService = approverService;
        }

        [HttpGet]
        public IActionResult Get([FromUri] WorkTimeApprovalQuery query)
        {
            var response = approverService.GetApprovers(query);

            return this.CreateResponse(response);
        }
    }
}
