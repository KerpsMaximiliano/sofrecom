using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Services.Common;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Sofco.WebApi.Controllers.AdvancementAndRefund
{
    [Route("api/refund")]
    [Authorize]
    public class RefundController : Controller
    {
        private readonly IRefundService refundService;
        private readonly IWorkflowService workflowService;
        private readonly IAdvancementService advancementService;
        private readonly IFileService fileService;
        private readonly FileConfig fileConfig;

        public RefundController(IRefundService refundService,
            IWorkflowService workflowService,
            IOptions<FileConfig> fileOptions,
            IAdvancementService advancementService,
            IFileService fileService)
        {
            this.refundService = refundService;
            this.workflowService = workflowService;
            this.advancementService = advancementService;
            this.fileService = fileService;
            fileConfig = fileOptions.Value;
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
            var response = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel { MustDoNextTransition = true } };

            while (response.Data.MustDoNextTransition)
            {
                workflowService.DoTransition<Refund, RefundHistory>(parameters, response);
            }

            var finalResponse = new Response();
            finalResponse.Messages = response.Messages;

            return this.CreateResponse(finalResponse);
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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = refundService.Delete(id);

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

        [HttpPost("search")]
        public IActionResult GetList([FromBody] RefundListParameterModel model)
        {
            var response = refundService.GetByParameters(model);

            return this.CreateResponse(response);
        }

        [HttpGet("export/{id}")]
        public IActionResult ExportFile(int id)
        {
            var response = fileService.ExportFile(id, fileConfig.RefundPath);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalitycs()
        {
            var response = refundService.GetAnalitycs();

            return this.CreateResponse(response);
        }

        [HttpPost("delegate/{userId}")]
        public IActionResult Delegate(int userId)
        {
            var response = refundService.Delegate(userId);

            return this.CreateResponse(response);
        }

        [HttpPost("delegate")]
        public IActionResult DeleteDelegate([FromBody] List<int> ids)
        {
            var response = refundService.DeleteDelegate(ids);

            return this.CreateResponse(response);
        }

        [HttpGet("delegates")]
        public IActionResult GetDelegates()
        {
            var response = refundService.GetDelegates();

            return this.CreateResponse(response);
        }
    }
}
