using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/employeenews")]
    public class EmployeeNewsController : Controller
    {
        private readonly IEmployeeService employeeService;

        public EmployeeNewsController(IEmployeeService employeeServ)
        {
            employeeService = employeeServ;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var response = employeeService.GetEmployeeNews();

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = employeeService.DeleteNews(id);

            return this.CreateResponse(response);
        }
    }
}
