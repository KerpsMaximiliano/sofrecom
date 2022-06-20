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
    [Route("api/RequestNoteFacturaPendienteAprobacionGerente")]
    public class RequestNoteFacturaPendienteAprobacionGerenteController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteFacturaPendienteAprobacionGerenteController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        [HttpPost("AprobarRecibidoConforme")]
        public IActionResult AprobarRecibidoConforme(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.PendienteProcesarGAF);

            return Ok();
        }
    }
}