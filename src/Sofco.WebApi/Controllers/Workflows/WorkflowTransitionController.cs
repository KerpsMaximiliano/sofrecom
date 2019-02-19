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

        [HttpPost]
        public IActionResult Post([FromBody] WorkflowTransitionAddModel model)
        {
            var response = workflowTransitionService.Post(model);

            return this.CreateResponse(response);
        }
    }
}
