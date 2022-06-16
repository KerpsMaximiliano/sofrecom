using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteAprobada")]
    public class RequestNoteAprobadaController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;
        private readonly IRequestNoteAnalitycService _requestNoteAnalitycService;

        public RequestNoteAprobadaController(IRequestNoteService requestNoteService, IRequestNoteAnalitycService requestNoteAnalityc)
        {
            this._requestNoteService = requestNoteService;
            this._requestNoteAnalitycService = requestNoteAnalityc;
        }

        [HttpPost("AprobarRequestNote")]
        protected IActionResult AprobarRequestNote(int id)
        {
            this._requestNoteService.CambiarAPendienteApobacionGerenteAnalitica(id);
            this._requestNoteAnalitycService.CambiarAPendienteAprobacion(id);

            return Ok();
        }

        [HttpPost("RechazarRequestNode")]
        protected IActionResult RechazarRequestNode(int id)
        {
            this._requestNoteService.RechazarRequestNote(id);
            this._requestNoteAnalitycService.Rechazar(id);

            return Ok();
        }

        [HttpPost("GuardarBorrador")]
        protected IActionResult GuardarBorrador([FromBody] RequestNoteSubmitBorradorDTO requestNote)
        {
            this._requestNoteService.GuardarBorrador(requestNote);

            return Ok();
        }

        [HttpGet("GetById")]
        protected IActionResult GetById(int id)
        {
            return Ok(this._requestNoteService.GetById(id));
        }

        [HttpGet("GetAll")]
        protected IActionResult GetAll(int id)
        {
            return Ok(this._requestNoteService.GetAll());
        }
    }
}