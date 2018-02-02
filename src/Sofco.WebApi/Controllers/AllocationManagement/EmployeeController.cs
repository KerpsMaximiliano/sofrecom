using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;
using Sofco.WebApi.Models.AllocationManagement;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employees")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;

        public EmployeeController(IEmployeeService employeeServ)
        {
            employeeService = employeeServ;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = employeeService.GetAll().Select(x => new EmployeeViewModel(x));

            return Ok(model);
        }

        [HttpGet("{id}/profile")]
        public IActionResult GetProfile(int id)
        {
            var response = employeeService.GetProfile(id);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = employeeService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] EmployeeSearchParams parameters)
        {
            var searchResponse = employeeService.Search(parameters);

            var response =
                new Response<IEnumerable<EmployeeViewModel>>
                {
                    Data = searchResponse.Data.Select(x => new EmployeeViewModel(x)),
                    Messages = searchResponse.Messages
                };

            return this.CreateResponse(response);
        }

        [HttpPost("sendUnsubscribeNotification/{employeeName}")]
        public IActionResult SendUnsubscribeNotification(string employeeName, [FromBody] UnsubscribeNotificationParams parameters)
        {
            parameters.UserName = this.GetUserName();

            var response = employeeService.SendUnsubscribeNotification(employeeName, parameters);

            return this.CreateResponse(response);
        }
    }
}
