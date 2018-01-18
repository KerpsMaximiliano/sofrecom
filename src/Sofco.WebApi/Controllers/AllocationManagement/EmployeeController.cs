using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Model.DTO;
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
            var model = employeeService.Search(parameters).Select(x => new EmployeeViewModel(x));

            return Ok(model);
        }


        [HttpPost("{newsId}")]
        public IActionResult Post(int newsId)
        {
            var response = employeeService.Add(newsId, this.GetUserName());

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("{newsId}")]
        public IActionResult Delete(int newsId)
        {
            var response = employeeService.Delete(newsId, this.GetUserName());

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("sendUnsubscribeNotification/{employeeName}")]
        public IActionResult SendUnsubscribeNotification(string employeeName)
        {
            var response = employeeService.SendUnsubscribeNotification(employeeName);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
