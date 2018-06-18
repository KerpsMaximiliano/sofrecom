using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/workTimeApprovals/employees")]
    [Authorize]
    public class WorkTimeApprovalsEmployeesController : Controller
    {
        private readonly IWorkTimeApprovalEmployeeService service;

        public WorkTimeApprovalsEmployeesController(IWorkTimeApprovalEmployeeService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get([FromUri] WorkTimeApprovalQuery query)
        {
            var response = service.Get(query);

            return this.CreateResponse(response);
        }
    }
}
