using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Models.AdvancementAndRefund;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;
using System.Collections.Generic;

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

        [HttpPost("transition")]
        public IActionResult DoTransition([FromBody] IList<WorkflowChangeStatusMasiveParameters> parameters)
        {
            var finalResponse = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel() };

            foreach (var parameter in parameters)
            {
                var response = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel() };

                if (parameter.Type == "refund")
                {
                    workflowService.DoTransition<Refund, RefundHistory>(parameter, response);

                    if (response.HasErrors())
                    {
                        var msg = string.Format(Resources.AdvancementAndRefund.Refund.PaymentPendingError, parameter.EntityId, parameter.UserApplicantName);
                        finalResponse.AddErrorAndNoTraslate(msg);
                    }
                }
                else if (parameter.Type == "advancement")
                {
                    workflowService.DoTransition<Advancement, AdvancementHistory>(parameter, response);

                    if (response.HasErrors())
                    {
                        var msg = string.Format(Resources.AdvancementAndRefund.Advancement.PaymentPendingError, parameter.EntityId, parameter.UserApplicantName);
                        finalResponse.AddErrorAndNoTraslate(msg);
                    }
                }
            }

            if (!finalResponse.HasErrors())
            {
                finalResponse.AddSuccess(Resources.Workflow.Workflow.TransitionSuccess);
            }

            return this.CreateResponse(finalResponse);
        }
    }
}
