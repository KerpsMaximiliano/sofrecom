using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Common;
using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Models.RequestNote;
using Sofco.Domain.Utils;
using Sofco.Service.Implementations.RequestNote;
using Sofco.WebApi.Extensions;

namespace Sofco.WebApi.Controllers.RequestNote
{
    [Route("api/RequestNoteRecibidoConforme")]
    public class RequestNoteRecibidoConformeController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;
        private readonly IFileService _fileService;
        private readonly FileConfig fileConfig;
        private readonly IRequestNoteProviderService _requestNoteProviderService;

        public RequestNoteRecibidoConformeController(IRequestNoteService requestNoteService, IFileService fileService, IOptions<FileConfig> fileOptions, IRequestNoteProviderService requestNoteProviderService)
        {
            this._requestNoteService = requestNoteService;
            this._fileService = fileService;
            this.fileConfig = fileOptions.Value;
            this._requestNoteProviderService = requestNoteProviderService;
        }

        [HttpPost("AprobarRecibidoConforme")]
        public IActionResult AprobarRecibidoConforme(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.FacturaPendienteAprobaciónGerente);

            return Ok();
        }

        [HttpPost("CerrarSolicitadaProveedor")]
        public IActionResult CerradaSolicitadaProveedor(int id)
        {
            this._requestNoteService.ChangeStatus(id, Domain.RequestNoteStates.RequestNoteStates.Cerrada);
            return Ok();
        }

        [HttpGet("DescargarArchivo")]
        public IActionResult DescargarArchivo(int fileId)
        {
            return Ok(this._fileService.GetFile(fileId, fileConfig.RefundPath));
        }

        [HttpGet("ListarArchivos")]
        public IActionResult ListarArchivos(int providerId)
        {
            return Ok(this._requestNoteProviderService.GetFilesByProviderId(providerId));
        }

        [HttpPost("CargarArchivosAdjuntos")]
        public IActionResult CargarArchivosAdjuntos([FromBody] List<RequestNoteProvider> requestNoteProviders)
        {
            this._requestNoteProviderService.GuardarArchivosProvider(requestNoteProviders);
            return Ok();
        }
    }
}