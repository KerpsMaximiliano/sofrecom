using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/task")]
    [Authorize]
    public class TaskController : Controller
    {
        private readonly ITaskService taskService;

        public TaskController(ITaskService taskService)
        {
            this.taskService = taskService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = taskService.GetAll(false);

            return Ok(list.Select(x => new TaskListItem(x)));
        }

        [HttpGet("options")]
        public IActionResult GetOptions()
        {
            var list = taskService.GetAll(true);

            return Ok(list.Select(x => new TaskOptionModel { Id = x.Id, Text = x.Description, CategoryId = x.CategoryId }));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = taskService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] TaskModel model)
        {
            var response = this.taskService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] TaskModel model)
        {
            var response = this.taskService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = this.taskService.Active(id, active);

            return this.CreateResponse(response);
        }
    }
}
