using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.Services.Common;
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
        private readonly IFileService _fileService;
        private readonly FileConfig fileConfig;
        private readonly IRequestNoteProviderService _requestNoteProviderService;

        public RequestNoteFacturaPendienteAprobacionGerenteController(IRequestNoteService requestNoteService, IFileService fileService, IOptions<FileConfig> fileOptions, IRequestNoteProviderService requestNoteProviderService)
        {
            this._requestNoteService = requestNoteService;
            this._fileService = fileService;
            this.fileConfig = fileOptions.Value;
            this._requestNoteProviderService = requestNoteProviderService;
        }

        

        [HttpGet("ListarArchivos")]
        public IActionResult ListarArchivos(int providerId)
        {
            return Ok(this._requestNoteProviderService.GetFilesByProviderId(providerId));
        }

        [HttpGet("DescargarArchivo")]
        public IActionResult DescargarArchivo(int fileId)
        {
            return Ok(this._fileService.GetFile(fileId, fileConfig.RefundPath));
        }
    }
}