using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/workTimeApprovals/employees")]
    [Authorize]
    public class EmployeeWorkTimeApprovalsController : Controller
    {
        private readonly IEmployeeWorkTimeApprovalService service;

        public EmployeeWorkTimeApprovalsController(IEmployeeWorkTimeApprovalService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = service.Get();

            return this.CreateResponse(response);
        }
    }
}
