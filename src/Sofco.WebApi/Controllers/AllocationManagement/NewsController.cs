using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.AllocationManagement;
using Sofco.WebApi.Models.AllocationManagement;

namespace Sofco.WebApi.Controllers.AllocationManagement
{
    [Authorize]
    [Route("api/news")]
    public class NewsController : Controller
    {
        private readonly IEmployeeService employeeService;

        public NewsController(IEmployeeService employeeServ)
        {
            employeeService = employeeServ;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            var news = employeeService.GetNews();

            var newsModel = news.Select(x => new NewsViewModel(x));

            return Ok(newsModel);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = employeeService.DeleteNews(id);

            if (response.HasErrors()) return BadRequest(response);

            return Ok(response);
        }
    }
}
