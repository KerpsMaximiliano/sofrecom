using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Workflow;

namespace Sofco.WebApi.Controllers.Workflows
{
    [Route("api/workflowstate")]
    [Authorize]
    public class WorkflowStateController : Controller
    {
        private readonly IWorkflowStateService stateService;

        public WorkflowStateController(IWorkflowStateService stateService)
        {
            this.stateService = stateService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var list = stateService.GetAll();

            return Ok(list.Select(x => new { Name = x.Name, ActionName = x.ActionName }));
        }
    }
}