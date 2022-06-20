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
    [Route("api/RequestNotePendienteRevisionAbastecimiento")]
    public class RequestNotePendienteRevisionAbastecimientoController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;
        private readonly IRequestNoteAnalitycService _requestNoteAnalitycService;

        public RequestNotePendienteRevisionAbastecimientoController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("AprobarPendienteRevisionAbastecimiento")]
        public IActionResult AprobarBorrador(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteAprobaciónGerentesAnalítica);
            this._requestNoteAnalitycService.ChangeStatusByRequestNodeId(id, "Pendiende Aprobación");

            return Ok();
        }

        [HttpPost("RechazarPendienteRevisionAbastecimiento")]
        public IActionResult RechazarBorrador(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.Reachazada);

            return Ok();
        }
    }
}