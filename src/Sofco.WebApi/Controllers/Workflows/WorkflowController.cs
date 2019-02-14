using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.Workflow;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.Workflows
{
    [Route("api/workflows")]
    [Authorize]
    public class WorkflowController : Controller
    {
        private readonly IWorkflowService workflowService;

        public WorkflowController(IWorkflowService workflowService)
        {
            this.workflowService = workflowService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var response = workflowService.GetAll();

            return this.CreateResponse(response);
        }

        [HttpGet("{workflowId}")]
        public IActionResult Get(int workflowId)
        {
            var response = workflowService.GetById(workflowId);

            return this.CreateResponse(response);
        }
    }
}
