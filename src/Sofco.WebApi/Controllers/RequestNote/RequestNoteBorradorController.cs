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
    [Route("api/RequestNoteBorrador")]
    public class RequestNoteBorradorController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteBorradorController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("GuardarBorrador")]
        public IActionResult GuardarBorrador([FromBody] Domain.Models.RequestNote.RequestNote requestNote)
        {
            this._requestNoteService.GuardarBorrador(requestNote);

            return Ok();
        }

        [HttpPost("AprobarBorrador")]
        public IActionResult AprobarBorrador(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteRevisiónAbastecimiento);

            return Ok();
        }

        [HttpPost("RechazarBorrador")]
        public IActionResult RechazarBorrador(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.Reachazada);

            return Ok();
        }

        [HttpGet("GetById")]
        public IActionResult GetById(int id)
        {
            return Ok(this._requestNoteService.GetById(id));
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll(int id)
        {
            return Ok(this._requestNoteService.GetAll());
        }
    }
}