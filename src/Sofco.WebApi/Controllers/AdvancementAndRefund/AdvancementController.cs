using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
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

        [HttpGet("inProcess")]
        public IActionResult GetAll()
        {
            var response = advancementService.GetAllInProcess();

            return this.CreateResponse(response);
        }

        [HttpPost("finalized")]
        public IActionResult GetAllFinalized([FromBody] AdvancementSearchFinalizedModel model)
        {
            var response = advancementService.GetAllFinalized(model);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = advancementService.Get(id);

            return this.CreateResponse(response);
        }

        [HttpGet("unrelated")]
        public IActionResult GetUnrelated(int id)
        {
            var response = advancementService.GetUnrelated();

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

        [HttpPost("transition")]
        public IActionResult DoTransition([FromBody] WorkflowChangeStatusParameters parameters)
        {
            var response = workflowService.DoTransition<Advancement, AdvancementHistory>(parameters);

            return this.CreateResponse(response);
        }

        [HttpPost("possibleTransitions")]
        public IActionResult GetPossibleTransitions([FromBody] TransitionParameters parameters)
        {
            var response = workflowService.GetPossibleTransitions<Advancement>(parameters);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var response = advancementService.GetHistories(id);

            return this.CreateResponse(response);
        }

        [HttpGet("canLoad")]
        public IActionResult CanLoad()
        {
            var response = advancementService.CanLoad();

            return this.CreateResponse(response);
        }
    }
}
