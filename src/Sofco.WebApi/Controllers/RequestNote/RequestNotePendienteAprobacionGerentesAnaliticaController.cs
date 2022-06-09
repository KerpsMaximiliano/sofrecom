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
    public class RequestNotePendienteAprobacionGerentesAnaliticaController : RequestNoteAbstractWorkflowController<RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO, RequestNoteLoadPendienteAprobacionGerentesAnaliticaDTO>
    {
        private readonly IRequestNoteService _requestNoteService;

        public RequestNotePendienteAprobacionGerentesAnaliticaController(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        protected override Response<RequestNoteLoadPendienteAprobacionGerentesAnaliticaDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO>> GetActionDictionary()
        {
            Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO>> map =
                new Dictionary<string, IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO>>
                {
                    { "Aprobar", new RequestNoteAprobarPendienteAprobacionGerentesAnaliticaCommand(_requestNoteService) },
                    { "Rechazar", new RequestNoteRechazarPendienteAprobacionGerentesAnaliticaCommand(_requestNoteService) }
                };
            return map;
        }
    }
}