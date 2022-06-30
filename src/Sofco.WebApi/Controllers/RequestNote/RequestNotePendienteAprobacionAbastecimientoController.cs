using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Models.RequestNote;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNotePendienteAprobacionAbastecimiento")]
    public class RequestNotePendienteAprobacionAbastecimientoController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;
        private readonly IRequestNoteProviderService _requestNoteProviderService;

        public RequestNotePendienteAprobacionAbastecimientoController(IRequestNoteService requestNoteService, IRequestNoteProviderService requestNoteProviderService)
        {
            this._requestNoteService = requestNoteService;
            this._requestNoteProviderService = requestNoteProviderService;
        }

        
        [HttpPost("AdjuntarArchivo")]
        public IActionResult AdjuntarArchivo(RequestNoteProvider requestNoteProvider)
        {
            this._requestNoteProviderService.GuardarArchivo(requestNoteProvider);

            return Ok();
        }
    }
}