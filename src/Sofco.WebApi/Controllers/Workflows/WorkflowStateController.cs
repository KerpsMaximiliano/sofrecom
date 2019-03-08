using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;

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

            return Ok(list.Select(x => new WorkflowStateListItem(x))
                          .OrderBy(x => x.Name));
        }

        [HttpGet("types")]
        public IActionResult GetTypes()
        {
            var list = stateService.GetTypes();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = stateService.GetById(id);

            return this.CreateResponse(response);
        }

        [HttpPut("{id}/active/{active}")]
        public IActionResult Active(int id, bool active)
        {
            var response = this.stateService.Active(id, active);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WorkflowStateModel model)
        {
            var response = this.stateService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] WorkflowStateModel model)
        {
            var response = this.stateService.Update(model);

            return this.CreateResponse(response);
        }
    }
}