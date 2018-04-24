using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/workTimeApprovals/approvers")]
    [Authorize]
    public class WorkTimeApprovalsApproversController : Controller
    {
        private readonly IApproversWorkTimeApprovalService service;

        public WorkTimeApprovalsApproversController(IApproversWorkTimeApprovalService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get([FromUri] WorkTimeApprovalQuery query)
        {
            var response = service.GetApprovers(query);

            return this.CreateResponse(response);
        }
    }
}
