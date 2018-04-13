using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
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

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = new List<Option>();

            options.AddRange(employeeService.GetAll().Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber} - {x.Name}" }));

            return Ok(options);
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
            var response = employeeService.SendUnsubscribeNotification(employeeName, parameters);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/finalizeExtraHolidays")]
        public IActionResult FinalizeExtraHolidays(int id)
        {
            var response = employeeService.FinalizeExtraHolidays(id);

            return this.CreateResponse(response);
        }

        [HttpPut("categories")]
        public IActionResult AddCategories([FromBody] EmployeeAddCategoriesParams parameters)
        {
            var response = employeeService.AddCategories(parameters);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/analytics")]
        public IActionResult GetAnalytics(int id)
        {
            return Ok(employeeService.GetAnalytics(id));
        }

        [HttpGet("{id}/categories")]
        public IActionResult GetCategories(int id)
        {
            var response = employeeService.GetCategories(id);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/businessHours")]
        public IActionResult UpdateBusinessHours(int id, [FromBody] EmployeeBusinessHoursParams model)
        {
            var response = employeeService.UpdateBusinessHours(id, model);

            return this.CreateResponse(response);
        }
    }
}
