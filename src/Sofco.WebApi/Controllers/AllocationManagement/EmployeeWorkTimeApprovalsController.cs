using System;
using System.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
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
        public IActionResult Get([FromUri] EmployeeWorkTimeApprovalQuery query)
        {
            var response = service.Get(query);

            return this.CreateResponse(response);
        }
    }
}
