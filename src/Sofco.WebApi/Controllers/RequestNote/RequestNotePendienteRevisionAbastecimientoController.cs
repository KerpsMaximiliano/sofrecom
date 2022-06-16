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
    public class RequestNotePendienteRevisionAbastecimientoController : RequestNoteAbstractWorkflowController<RequestNoteSubmitPendienteRevisionAbastecimientoDTO, RequestNoteLoadPendienteRevisionAbastecimientoDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteRevisionAbastecimientoController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadPendienteRevisionAbastecimientoDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteRevisionAbastecimientoDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteRevisionAbastecimientoDTO>> map =
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteRevisionAbastecimientoDTO>>
                {
                    { "Enviar", new RequestNoteEnviarPendienteRevisionAbastecimientoCommand(_requestNoteService) },
                    { "Rechazar", new RequestNoteRechazarPendienteRevisionAbastecimientoCommand(_requestNoteService) }
                };
            return map;
        }
    }
}