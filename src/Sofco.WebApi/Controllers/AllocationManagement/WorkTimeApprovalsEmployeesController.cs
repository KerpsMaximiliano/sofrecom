using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Route("api/workTimeApprovals/employees")]
    [Authorize]
    public class WorkTimeApprovalsEmployeesController : Controller
    {
        private readonly IUserApproverEmployeeService service;

        public WorkTimeApprovalsEmployeesController(IUserApproverEmployeeService service)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Get([FromUri] UserApproverQuery query)
        {
            var response = service.Get(query, UserApproverType.WorkTime);

            return this.CreateResponse(response);
        }
    }
}
