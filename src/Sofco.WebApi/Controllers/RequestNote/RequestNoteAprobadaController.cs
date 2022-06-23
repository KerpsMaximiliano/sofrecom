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
    [Route("api/RequestNoteAprobada")]
    public class RequestNoteAprobadaController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;
        private readonly IRequestNoteAnalitycService _requestNoteAnalitycService;
        private readonly IRequestNoteProviderService _requestNoteProviderService;

        public RequestNoteAprobadaController(IRequestNoteService requestNoteService, IRequestNoteAnalitycService requestNoteAnalityc, IRequestNoteProviderService requestNoteProviderService)
        {
            this._requestNoteService = requestNoteService;
            this._requestNoteAnalitycService = requestNoteAnalityc;
            this._requestNoteProviderService = requestNoteProviderService;
        }

        [HttpPost("AprobarAprobada")]
        public IActionResult AprobarRequestNote(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.SolicitadaAProveedor);

            return Ok();
        }

        [HttpPost("RechazarAprobada")]
        public IActionResult RechazarRequestNode(int id)
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

        [HttpGet("ListarArchivos")]
        public IActionResult ListarArchivos(int providerId)
        {
            return Ok(this._requestNoteProviderService.GetFilesByProviderId(providerId));
        }

        [HttpPost("CargarArchivosProveedor")]
        public IActionResult CargarArchivosProveedor([FromBody] List<RequestNoteProvider> requestNoteProviders)
        {
            this._requestNoteProviderService.GuardarArchivosProvider(requestNoteProviders);
            return Ok();
        }
    }
}