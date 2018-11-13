using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employees")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService employeeService;
        private readonly IUserService userService;

        public EmployeeController(IEmployeeService employeeServ, IUserService userService)
        {
            employeeService = employeeServ;
            this.userService = userService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var model = employeeService.GetAll().Select(x => new EmployeeModel(x));

            return Ok(model.OrderBy(x => x.Name));
        }

        [HttpPost("search/unemployees")]
        public IActionResult GetUnemployees([FromBody] UnemployeeSearchParameters parameters)
        {
            var model = employeeService.GetUnemployees(parameters);

            return Ok(model);
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = new List<Option>();

            options.AddRange(employeeService.GetAll().OrderBy(x => x.Name).Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber} - {x.Name}" }));

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

        [HttpGet("{id}/info")]
        public IActionResult GetInfo(int id)
        {
            var response = userService.GetUserInfo(id);

            return this.CreateResponse(response);
        }

        [HttpPost("search")]
        public IActionResult Search([FromBody] EmployeeSearchParams parameters)
        {
            var searchResponse = employeeService.Search(parameters);

            var response =
                new Response<IEnumerable<EmployeeModel>>
                {
                    Data = searchResponse.Data.Select(x => new EmployeeModel(x)),
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

        [HttpGet("currentCategories")]
        public IActionResult GetCurrentCategories()
        {
            var response = employeeService.GetCurrentCategories();

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/pendingWorkingHours")]
        public IActionResult PendingWorkingHours(int id)
        {
            var response = employeeService.GetPendingWorkingHours(id);

            return this.CreateResponse(response);
        }

        [HttpPost("external")]
        public IActionResult AddExternal([FromBody] AddExternalModel model)
        {
            var response = employeeService.AddExternal(model);

            return this.CreateResponse(response);
        }

        [HttpGet("currentManager/options")]
        public IActionResult GetOptionByCurrentManager()
        {
            var response = employeeService.GetEmployeeOptionByCurrentManager();

            return this.CreateResponse(response);
        }
    }
}
