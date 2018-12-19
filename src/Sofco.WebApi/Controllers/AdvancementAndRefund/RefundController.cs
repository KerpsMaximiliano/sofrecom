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

        public RefundController(IRefundService refundService, IWorkflowService workflowService)
        {
            this.refundService = refundService;
            this.workflowService = workflowService;
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

        [HttpGet("states")]
        public IActionResult Get()
        {
            var response = refundService.GetStates();

            return this.CreateResponse(response);
        }
    }
}
