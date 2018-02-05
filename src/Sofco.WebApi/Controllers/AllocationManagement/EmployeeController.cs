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

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var options = new List<Option> { new Option { Id = 0, Text = "Seleccione una opcion" } };

            options.AddRange(employeeService.GetAll().Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber} - {x.Name}" }));

            return Ok(options);
        }

        [HttpGet("{id}/profile")]
        public IActionResult GetProfile(int id)
        {
            var response = employeeService.GetProfile(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = employeeService.GetById(id);

            if (response.HasErrors())
                return BadRequest(response);

            return Ok(new EmployeeViewModel(response.Data));
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

            return Ok(response);
        }

        [HttpPost("sendUnsubscribeNotification/{employeeName}")]
        public IActionResult SendUnsubscribeNotification(string employeeName, [FromBody] UnsubscribeNotificationParams parameters)
        {
            parameters.UserName = this.GetUserName();

            var response = employeeService.SendUnsubscribeNotification(employeeName, parameters);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
