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
    [Route("api/RequestNotePendienteAprobacionAbastecimiento")]
    public class RequestNotePendienteAprobacionAbastecimientoController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteAprobacionAbastecimientoController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("AprobarPendienteAprobacionAbastecimiento")]
        public IActionResult AprobarPendienteAprobacionAbastecimiento(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteAprobaciónDAF);

            return Ok();
        }

        [HttpPost("RechazarPendienteAprobacionAbastecimiento")]
        public IActionResult RechazarPendienteAprobacionAbastecimiento(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.Reachazada);

            return Ok();
        }
    }
}