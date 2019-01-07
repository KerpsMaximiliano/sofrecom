using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employees/endNotifications")]
    public class EmployeeEndNotificationController : Controller
    {
        private readonly IEmployeeEndNotificationService service;

        public EmployeeEndNotificationController(IEmployeeEndNotificationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public IActionResult Get([FromBody]EmployeeEndNotificationParameters parameters)
        {
            var response = service.Get(parameters);

            return this.CreateResponse(response);
        }
    }
}
