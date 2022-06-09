using Sofco.Core.Services.RequestNote;
using Sofco.Domain.DTO;
using Sofco.Domain.DTO.NotaPedido;
using Sofco.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sofco.Service.Implementations.RequestNote
{
    public class RequestNoteRechazarPendienteAprobacionGerentesAnaliticaCommand : IRequestNoteCommand<RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO>
    {

        private readonly IRequestNoteService _requestNoteService;

        public RequestNoteRechazarPendienteAprobacionGerentesAnaliticaCommand(IRequestNoteService requestNoteService)
        {
            this._requestNoteService = requestNoteService;
        }

        public Response CanExecute(RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Execute(RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Notify(RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO dto)
        {
            throw new NotImplementedException();
        }

        public Response Validate(RequestNoteSubmitPendienteAprobacionGerentesAnaliticaDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
