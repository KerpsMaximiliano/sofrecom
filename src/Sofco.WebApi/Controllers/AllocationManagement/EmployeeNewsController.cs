using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employees/news")]
    public class EmployeeNewsController : Controller
    {
        private readonly IEmployeeNewsService employeeNewsService;

        public EmployeeNewsController(IEmployeeNewsService employeeNewsService)
        {
            this.employeeNewsService = employeeNewsService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = employeeNewsService.GetEmployeeNews();

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = employeeNewsService.Delete(id, this.GetUserName());

            return this.CreateResponse(response);
        }

        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
            var response = employeeNewsService.Add(id, this.GetUserName());

            return this.CreateResponse(response);
        }

        [HttpPost("cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            var response = employeeNewsService.Cancel(id);

            return this.CreateResponse(response);
        }
    }
}
