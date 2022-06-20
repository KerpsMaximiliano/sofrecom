﻿using System;
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
    [Route("api/RequestNotePendienteAprobacionGerentesAnalitica")]
    public class RequestNotePendienteAprobacionGerentesAnaliticaController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteAprobacionGerentesAnaliticaController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("AprobarPendienteAprobacionGerentesAnalitica")]
        public IActionResult PasarPendienteAprobacionAbastecimiento(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteAprobaciónAbastecimiento);

            return Ok();
        }

        [HttpPost("RechazarPendienteAprobacionGerentesAnalitica")]
        public IActionResult RechazarPendienteAprobacionGerentesAnalitica(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteRevisiónAbastecimiento);

            return Ok();
        }
    }
}