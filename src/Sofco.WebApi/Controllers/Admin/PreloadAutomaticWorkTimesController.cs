using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Admin
{
    [Route("api/PreloadAutomaticWorkTimes")]
    [Authorize]
    public class PreloadAutomaticWorkTimesController : Controller
    {
        private readonly IEmployeeWorkTimesAddJobService employeeWorkTimesAddJobService;
        private readonly ITaskService taskService;


        public PreloadAutomaticWorkTimesController(IEmployeeWorkTimesAddJobService employeeWorkTimesAddJobService, ISettingService settingService, ITaskService taskService)
        {
            this.employeeWorkTimesAddJobService = employeeWorkTimesAddJobService;
            this.taskService = taskService;
        }
        [HttpGet("GetTasks")]
        public IActionResult GetTasks()
        {
            var list = taskService.GetByCategory("Proyectos");
            return Ok(list);
        }

        [HttpGet("RunProcess")]
        public IActionResult RunProcess()
        {
            this.employeeWorkTimesAddJobService.Run();
            return Ok();
        }
    }
}
