using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Models.RequestNote;
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
        private readonly IRequestNoteProviderService _requestNoteProviderService;

        public RequestNotePendienteRevisionAbastecimientoController(IRequestNoteService requestNoteService, IRequestNoteAnalitycService requestNoteAnalitycService, IRequestNoteProviderService requestNoteProviderService)
        {
            this._requestNoteService = requestNoteService;
            this._requestNoteAnalitycService = requestNoteAnalitycService;
            this._requestNoteProviderService = requestNoteProviderService;
        }

        [HttpPost("GuardarArchivo")]
        public IActionResult GuardarArchivo([FromBody] RequestNoteProvider requestNoteProvider)
        {
            this._requestNoteProviderService.Save(requestNoteProvider);
            return Ok();
        }
    }
}