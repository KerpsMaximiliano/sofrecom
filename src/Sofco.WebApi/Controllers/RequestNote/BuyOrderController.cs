using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.Models.BuyOrder;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.RequestNote;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.Utils;
using Sofco.WebApi.Extensions;
using System.Web.Http;

namespace Sofco.WebApi.Controllers.PurchaseOrders
{
    [Route("api/buyOrderRequestNote")]
    public class BuyOrderController : Controller
    {
        private readonly IBuyOrderService service;

        private readonly IRequestNoteService requestNoteService;

        private readonly IWorkflowService workflowService;

        private readonly AppSetting settings;

        public BuyOrderController(IBuyOrderService service, IWorkflowService workflowService, IOptions<AppSetting> settingOptions, IRequestNoteService requestNoteService)
        {
            this.service = service;
            this.workflowService = workflowService;
            settings = settingOptions.Value;
            this.requestNoteService = requestNoteService;
        }

        [HttpGet("States")]
        public IActionResult Get()
        {
            var response = service.GetStates();

            return this.CreateResponse(response);
        }

        [HttpPost("possibleTransitions")]
        public IActionResult GetPossibleTransitions([FromBody] TransitionParameters parameters)
        {
            // parameters.WorkflowId = settings.BuyOrderWorkflowId;
            var response = workflowService.GetPossibleTransitions<Sofco.Domain.Models.RequestNote.BuyOrder>(parameters);

            return this.CreateResponse(response);
        }

        [HttpPost("transition")]
        public IActionResult DoTransition([FromBody] WorkflowChangeBuyOrderParameters parameters)
        {
            if (parameters?.BuyOrder == null)
            {
                var r = new Response<int>();
                r.AddError(Resources.RequestNote.RequestNote.NullModel);
                return this.CreateResponse(r);
            }

            this.service.SaveChanges(parameters?.BuyOrder, parameters.NextStateId);

            var response = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel { MustDoNextTransition = true } };

            while (response.Data.MustDoNextTransition)
            {
                workflowService.DoTransition<Domain.Models.RequestNote.BuyOrder, Domain.Models.RequestNote.BuyOrderHistory>(parameters, response);
            }

            // Si anduvo ok y nos dice que hay que cambiar el estado de la NP => hay que invocar al workflow de NP
            if (!response.HasErrors() && parameters.NextStateIdRequestNote.HasValue)
            {
                response = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel { MustDoNextTransition = true } };
                var order = this.service.GetById(parameters.EntityId).Data;
                var note = requestNoteService.GetById(order.RequestNoteId).Data;
                if (note.StatusId != parameters.NextStateIdRequestNote)
                {
                    var parNP = new WorkflowChangeRequestNoteParameters()
                    {
                        EntityId = note.Id.Value,
                        RequestNote = note,
                        NextStateId = parameters.NextStateIdRequestNote.Value,
                        WorkflowId = note.WorkflowId
                    };
                    while (response.Data.MustDoNextTransition)
                    {
                        workflowService.DoTransition<Domain.Models.RequestNote.RequestNote, Domain.Models.RequestNote.RequestNoteHistory>(parNP, response);
                    }
                }
            }

            return this.CreateResponse(response);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            return Ok(this.service.GetById(id));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll([FromUri] BuyOrderGridFilters filters)
        {
            return Ok(this.service.GetAll(filters));
        }

        [HttpPost]
        public IActionResult Post([FromBody] BuyOrderModel model)
        {
            var response = service.Add(model);

            return this.CreateResponse(response);
        }
    }
}
