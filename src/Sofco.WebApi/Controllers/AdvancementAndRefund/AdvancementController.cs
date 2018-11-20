using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/advancement")]
    [Authorize]
    public class AdvancementController : Controller
    {
        private readonly IAdvancementService advancementService;
        private readonly IWorkflowService workflowService;

        public AdvancementController(IAdvancementService advancementService, IWorkflowService workflowService)
        {
            this.advancementService = advancementService;
            this.workflowService = workflowService;
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = advancementService.Get(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] AdvancementModel model)
        {
            var response = advancementService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] AdvancementModel model)
        {
            var response = advancementService.Update(model);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult DoTransition(WorkflowChangeStatusParameters parameters)
        {
            var response = workflowService.DoTransition<Advancement>(parameters);

            return this.CreateResponse(response);
        }
    }
}
