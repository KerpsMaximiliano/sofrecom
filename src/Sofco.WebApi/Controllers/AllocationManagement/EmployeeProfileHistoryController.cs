using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employeeProfileHistories")]
    public class EmployeeProfileHistoryController : Controller
    {
        private readonly IEmployeeProfileHistoryService service;

        public EmployeeProfileHistoryController(IEmployeeProfileHistoryService service)
        {
            this.service = service;
        }

        [HttpGet("{employeeNumber}")]
        public IActionResult GetById(string employeeNumber)
        {
            var response = service.GetByEmployeeNumber(employeeNumber);

            return this.CreateResponse(response);
        }
    }
}
