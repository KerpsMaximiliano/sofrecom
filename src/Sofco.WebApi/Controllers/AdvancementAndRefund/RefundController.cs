using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = refundService.Get(id);

            return this.CreateResponse(response);
        }

        [HttpPost]
        public IActionResult Post([FromBody] RefundModel model)
        {
            var response = refundService.Add(model);

            return this.CreateResponse(response);
        }

        [HttpPut]
        public IActionResult Put([FromBody] RefundModel model)
        {
            var response = refundService.Update(model);

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

        [HttpPost("{refundId}/file")]
        public async Task<IActionResult> File(int refundId)
        {
            var response = new Response<File>();

            if (Request.Form.Files.Any())
            {
                var file = Request.Form.Files.First();

                await refundService.AttachFile(refundId, response, file);
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpDelete("{id}/file/{fileId}")]
        public IActionResult DeleteFile(int id, int fileId)
        {
            var response = refundService.DeleteFile(id, fileId);

            return this.CreateResponse(response);
        }

        [HttpGet("{id}/histories")]
        public IActionResult GetHistories(int id)
        {
            var response = refundService.GetHistories(id);

            return this.CreateResponse(response);
        }

        [HttpGet("states")]
        public IActionResult Get()
        {
            var response = refundService.GetStates();

            return this.CreateResponse(response);
        }

        [HttpPost("/list")]
        public IActionResult GetList([FromBody] RefundListParameterModel model)
        {
            var response = refundService.GetByParameters(model);

            return this.CreateResponse(response);
        }
    }
}
