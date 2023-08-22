using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Models.RequestNote;
using Sofco.Core.Models.Workflow;
using Sofco.Core.Services.RequestNote;
using Sofco.Core.Services.Workflow;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.Common;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;
using File = Sofco.Domain.Models.Common.File;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteBorrador")]
    [ApiController]
    public class RequestNoteController : Controller
    {
        private readonly IRequestNoteService _requestNoteService;

        private readonly IWorkflowService workflowService;

        public RequestNoteController(IRequestNoteService requestNoteService, IWorkflowService workflowService)
        {
            this._requestNoteService = requestNoteService;
            this.workflowService = workflowService;
        }

        [HttpPost("possibleTransitions")]
        public IActionResult GetPossibleTransitions([FromBody] TransitionParameters parameters)
        {
            var response = workflowService.GetPossibleTransitions<Sofco.Domain.Models.RequestNote.RequestNote>(parameters);

            return this.CreateResponse(response);
        }

        [HttpPost("transition")]
        public IActionResult DoTransition([FromBody] WorkflowChangeRequestNoteParameters parameters)
        {
            if (parameters?.RequestNote == null && !parameters.Parameters.ContainsKey("comments"))
            {
                var r = new Response<int>();
                r.AddError(Resources.RequestNote.RequestNote.NullModel);
                return this.CreateResponse(r);
            }

            if (parameters?.RequestNote != null) { 
                this._requestNoteService.SaveChanges(parameters?.RequestNote, parameters.NextStateId);
            }

            var response = new Response<TransitionSuccessModel> { Data = new TransitionSuccessModel { MustDoNextTransition = true } };

            while (response.Data.MustDoNextTransition)
            {
                workflowService.DoTransition<Domain.Models.RequestNote.RequestNote, Domain.Models.RequestNote.RequestNoteHistory>(parameters, response);
            }

            return this.CreateResponse(response);
        }

        [HttpPost("GuardarBorrador")]
        public IActionResult GuardarBorrador([FromBody] RequestNoteModel requestNote)
        {
            var response = this._requestNoteService.Add(requestNote);

            return this.CreateResponse(response);
        }

        [HttpGet("GetUnits")]
        public IActionResult GetUnits()
        {
            return Ok(_requestNoteService.GetUnits());
        }
        [HttpGet("GetCurrencies")]
        public IActionResult GetCurrencies()
        {
            return Ok(_requestNoteService.GetCurrencies());
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> UploadFiles()
        {
            var response = new Response<List<File>>();

            if (Request.Form.Files.Any())
            {
                await _requestNoteService.AttachFiles(response, Request.Form.Files.ToList());
            }
            else
            {
                response.AddError(Resources.Common.SaveFileError);
            }

            return this.CreateResponse(response);
        }

        [HttpGet("States")]
        public IActionResult Get()
        {
            var response = _requestNoteService.GetStates();

            return this.CreateResponse(response);
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            return Ok(this._requestNoteService.GetById(id));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll([FromUri] RequestNoteGridFilters filters)
        {
            return Ok(this._requestNoteService.GetAll(filters));
        }


        [HttpGet("GetProviders")]
        public IActionResult GetProviders(int requestNoteID)
        {
            return Ok(this._requestNoteService.GetProviders(requestNoteID));
        }
        /*
        [HttpPost("SaveChanges")]
        public IActionResult SaveChanges([FromBody] RequestNoteModel requestNote, int nextStatus)
        {
            this._requestNoteService.SaveChanges(requestNote, nextStatus);

            return Ok();
        }*/
    }
}