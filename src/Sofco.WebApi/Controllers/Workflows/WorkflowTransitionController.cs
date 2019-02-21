using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Workflows
{
    [Route("api/workflowtransitions")]
    [Authorize]
    public class WorkflowTransitionController : Controller
    {
        private readonly IWorkflowTransitionService workflowTransitionService;

        public WorkflowTransitionController(IWorkflowTransitionService workflowTransitionService)
        {
            this.workflowTransitionService = workflowTransitionService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = workflowTransitionService.Get(id);

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = workflowTransitionService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WorkflowTransitionAddModel model)
        {
            var response = workflowTransitionService.Post(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] WorkflowTransitionAddModel model)
        {
            var response = workflowTransitionService.Put(model);

            return this.CreateResponse(response);
        }
    }
}
