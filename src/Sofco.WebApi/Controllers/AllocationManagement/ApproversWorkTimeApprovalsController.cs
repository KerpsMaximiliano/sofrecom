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
    public class ApproversWorkTimeApprovalsController : Controller
    {
        private readonly IApproversWorkTimeApprovalService service;

        public ApproversWorkTimeApprovalsController(IApproversWorkTimeApprovalService service)
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
