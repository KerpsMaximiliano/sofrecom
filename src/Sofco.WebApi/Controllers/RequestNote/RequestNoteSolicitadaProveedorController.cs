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
    [Route("api/RequestNoteSolicitadaProveedor")]
    public class RequestNoteSolicitadaProveedorController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteSolicitadaProveedorController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("AprobarSolicitadaProveedor")]
        public IActionResult AprobarSolicitadaProveedor(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.RecibidoConforme);

            return Ok();
        }

        [HttpPost("CerrarSolicitadaProveedor")]
        public IActionResult CerradaSolicitadaProveedor(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.Cerrada);

            return Ok();
        }
    }
}