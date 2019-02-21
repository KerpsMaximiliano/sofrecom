using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Workflow;
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

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] WorkflowAddModel model)
        {
            var response = workflowService.Put(id, model);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] WorkflowAddModel model)
        {
            var response = workflowService.Post(model);

            return this.CreateResponse(response);
        }

        [HttpGet("types")]
        public IActionResult GetTypes()
        {
            var response = workflowService.GetTypes();

            return this.CreateResponse(response);
        }

        [HttpGet("states")]
        public IActionResult GetStates()
        {
            var response = workflowService.GetStates();

            return this.CreateResponse(response);
        }
    }
}
