using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;

namespace Sofco.WebApi.Controllers.Workflow
{
    [Route("api/workflow")]
    [Authorize]
    public class WorkflowController : Controller
    {
        private readonly IWorkflowService workflowService;

        public WorkflowController(IWorkflowService workflowService)
        {
            this.workflowService = workflowService;
        }
    }
}
