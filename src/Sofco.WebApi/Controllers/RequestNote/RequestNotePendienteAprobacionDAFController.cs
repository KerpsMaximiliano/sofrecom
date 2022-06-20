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
    [Route("api/RequestNotePendienteAprobacionDAF")]
    public class RequestNotePendienteAprobacionDAFController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteAprobacionDAFController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("AprobarPendienteAprobacionDAF")]
        public IActionResult AprobarPendienteAprobacionDAF(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.Aprobada);

            return Ok();
        }

        [HttpPost("RechazarPendienteAprobacionDAF")]
        public IActionResult RechazarPendienteAprobacionDAF(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteAprobaciónAbastecimiento);

            return Ok();
        }
    }
}