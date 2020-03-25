using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.AdvancementAndRefund.Advancement;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.AdvancementAndRefund;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
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

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var response = advancementService.Delete(id);

            return this.CreateResponse(response);
        }

        [HttpGet("unrelated/{userId}")]
        public IActionResult GetUnrelated(int userId)
        {
            var response = advancementService.GetUnrelated(userId);

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
            var response = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel { MustDoNextTransition = true } };

            while (response.Data.MustDoNextTransition)
            {
                workflowService.DoTransition<Advancement, AdvancementHistory>(parameters, response);
            }

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

        [HttpPost("resume")]
        public IActionResult GetResume([FromBody] List<int> ids)
        {
            var response = advancementService.GetResume(ids);

            return this.CreateResponse(response);
        }

        [HttpGet("states")]
        public IActionResult Get()
        {
            var response = advancementService.GetStates();

            return this.CreateResponse(response);
        }

        [HttpGet("cashEnable")]
        public IActionResult IsCashEnable()
        {
            var response = advancementService.IsCashEnable();

            if (response.HasErrors())
                return BadRequest();
            else
                return Ok();
        }

        [HttpGet("{id}/export")]
        public IActionResult ExportFile(int id)
        {
            var response = advancementService.GetFile(id);

            if (response.HasErrors())
                return BadRequest(response);

            return File(response.Data, "application/octet-stream", string.Empty);
        }
    }
}
