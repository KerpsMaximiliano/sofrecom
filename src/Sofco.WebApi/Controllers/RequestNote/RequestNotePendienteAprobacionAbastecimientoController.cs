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
    public class RequestNotePendienteAprobacionAbastecimientoController : RequestNoteAbstractWorkflowController<RequestNoteSubmitPendienteAprobacionAbastecimientoDTO, RequestNoteLoadPendienteAprobacionAbastecimientoDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteAprobacionAbastecimientoController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadPendienteAprobacionAbastecimientoDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionAbastecimientoDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionAbastecimientoDTO>> map =
                 new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionAbastecimientoDTO>>
                 {
                    { "Aprobar", new RequestNoteAprobarPendienteAprobacionAbastecimientoCommand(_requestNoteService) },
                    { "Rechazar", new RequestNoteRechazarPendienteAprobacionAbastecimientoCommand(_requestNoteService) }
                 };
            return map;
        }
    }
}