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
    public class RequestNotePendienteAprobacionDAFController : RequestNoteAbstractWorkflowController<RequestNoteSubmitPendienteAprobacionDAFDTO, RequestNoteLoadPendienteAprobacionDAFDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteAprobacionDAFController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadPendienteAprobacionDAFDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionDAFDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionDAFDTO>> map =
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionDAFDTO>>
                {
                    { "Aprobar", new RequestNoteAprobarPendienteAprobacionDAFCommand(_requestNoteService) },
                    { "Rechazar", new RequestNoteRechazarPendienteAprobacionDAFCommand(_requestNoteService) }
                };
            return map;
        }
    }
}