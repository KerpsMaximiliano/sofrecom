using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/refund")]
    [Authorize]
    public class RefundController : Controller
    {
        private readonly IRefundService refundService;
        private readonly IWorkflowService workflowService;
        private readonly IAdvancementService advancementService;

        public RefundController(IRefundService refundService, IWorkflowService workflowService, IAdvancementService advancementService)
        {
            this.refundService = refundService;
            this.workflowService = workflowService;
            this.advancementService = advancementService;
        }

        [HttpPost]
        public IActionResult Post([FromBody] RefundModel model)
        {
            var response = refundService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPost("transition")]
        public IActionResult DoTransition([FromBody] WorkflowChangeStatusParameters parameters)
        {
            var response = workflowService.DoTransition<Refund, RefundHistory>(parameters);

            return this.CreateResponse(response);
        }

        [HttpPost("possibleTransitions")]
        public IActionResult GetPossibleTransitions([FromBody] TransitionParameters parameters)
        {
            var response = workflowService.GetPossibleTransitions<Refund>(parameters);

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
