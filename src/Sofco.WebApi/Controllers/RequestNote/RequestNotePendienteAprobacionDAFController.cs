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
    [Route("api/RequestNotePendienteAprobacionDAF")]
    public class RequestNotePendienteAprobacionDAFController : ControllerBase
    {
        private readonly IRequestNoteService _requestNoteService;
        private readonly IFileService _fileService;
        private readonly FileConfig fileConfig;

        public RequestNotePendienteAprobacionDAFController(IRequestNoteService requestNoteService, IFileService fileService, IOptions<FileConfig> fileOptions)
        {
            this._requestNoteService = requestNoteService;
            this._fileService = fileService;
            this.fileConfig = fileOptions.Value;
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

        [HttpPost("DescargarArchivo")]
        public IActionResult DescargarArchivo(int id, int type)
        {
            this._fileService.GetFile(id, GetPathUrl(type));

            return Ok();
        }

        private string GetPathUrl(int type)
        {
            switch (type)
            {
                case 1: return fileConfig.PurchaseOrdersPath;
                case 2: return fileConfig.CertificatesPath;
                case 3: return fileConfig.InvoicesPdfPath;
                case 4: return fileConfig.RecruitmentPath;
                default: return string.Empty;
            }
        }
    }
}